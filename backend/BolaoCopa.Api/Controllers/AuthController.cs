using System.Security.Claims;
using BolaoCopa.Api.Data;
using BolaoCopa.Api.DTOs;
using BolaoCopa.Api.Models;
using BolaoCopa.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolaoCopa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _passwordHasher;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext db, PasswordHasher passwordHasher, TokenService tokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
    {
        var name = NormalizeName(request.Name);
        var email = NormalizeEmail(request.Email);
        var password = request.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { message = "Informe o nome." });
        }

        if (password.Length < 6)
        {
            return BadRequest(new { message = "A senha deve ter pelo menos 6 caracteres." });
        }

        var nameLower = name.ToLowerInvariant();
        if (await _db.Users.AnyAsync(x => x.Name.ToLower() == nameLower))
        {
            return BadRequest(new { message = "Nome já cadastrado. Escolha outro nome para entrar no bolão." });
        }

        if (email is not null && await _db.Users.AnyAsync(x => x.Email == email))
        {
            return BadRequest(new { message = "E-mail já cadastrado." });
        }

        var isFirstUser = !await _db.Users.AnyAsync();

        var user = new AppUser
        {
            Name = name,
            Email = email,
            PasswordHash = _passwordHasher.Hash(password),
            Role = isFirstUser ? "admin" : "participant"
        };

        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return BadRequest(new { message = "Nome ou e-mail já cadastrado." });
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new { message = "Não foi possível criar o usuário no banco de dados." });
        }

        var token = _tokenService.Generate(user);
        return Ok(new LoginResponse(token, ToUserDto(user)));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var login = !string.IsNullOrWhiteSpace(request.Login)
            ? request.Login.Trim()
            : request.Email?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(login))
        {
            return BadRequest(new { message = "Informe seu nome ou e-mail." });
        }

        var loginLower = login.ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(x =>
            x.IsActive &&
            (x.Name.ToLower() == loginLower || (x.Email != null && x.Email.ToLower() == loginLower)));

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Nome/e-mail ou senha inválidos." });
        }

        var token = _tokenService.Generate(user);
        return Ok(new LoginResponse(token, ToUserDto(user)));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var id))
        {
            return Unauthorized();
        }

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(ToUserDto(user));
    }

    private static string NormalizeName(string? value) => (value ?? string.Empty).Trim();

    private static string? NormalizeEmail(string? value)
    {
        var email = value?.Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(email) ? null : email;
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        var current = ex.InnerException;
        while (current is not null)
        {
            if (current.GetType().FullName == "Npgsql.PostgresException")
            {
                var sqlState = current.GetType().GetProperty("SqlState")?.GetValue(current)?.ToString();
                return sqlState == "23505";
            }

            current = current.InnerException;
        }

        return false;
    }

    private static UserDto ToUserDto(AppUser user) => new(
        user.Id,
        user.Name,
        user.Email,
        user.Role,
        user.IsActive,
        user.CreatedAt
    );
}
