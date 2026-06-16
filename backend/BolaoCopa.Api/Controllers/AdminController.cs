using BolaoCopa.Api.Data;
using BolaoCopa.Api.DTOs;
using BolaoCopa.Api.Models;
using BolaoCopa.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolaoCopa.Api.Controllers;

[Authorize(Roles = "admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _passwordHasher;
    private readonly ScoringService _scoringService;

    public AdminController(AppDbContext db, PasswordHasher passwordHasher, ScoringService scoringService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _scoringService = scoringService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _db.Users
            .OrderBy(x => x.Name)
            .Select(x => new UserDto(x.Id, x.Name, x.Email, x.Role, x.IsActive, x.CreatedAt))
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("users")]
    public async Task<ActionResult<UserDto>> CreateUser(AdminCreateUserRequest request)
    {
        var name = NormalizeName(request.Name);
        var email = NormalizeEmail(request.Email);

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { message = "Informe o nome." });
        }

        if (request.Password.Length < 6)
        {
            return BadRequest(new { message = "A senha deve ter pelo menos 6 caracteres." });
        }

        var nameLower = name.ToLowerInvariant();
        if (await _db.Users.AnyAsync(x => x.Name.ToLower() == nameLower))
        {
            return BadRequest(new { message = "Nome já cadastrado." });
        }

        if (email is not null && await _db.Users.AnyAsync(x => x.Email == email))
        {
            return BadRequest(new { message = "E-mail já cadastrado." });
        }

        var role = request.Role.Equals("admin", StringComparison.OrdinalIgnoreCase)
            ? "admin"
            : "participant";

        var user = new AppUser
        {
            Name = name,
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new UserDto(user.Id, user.Name, user.Email, user.Role, user.IsActive, user.CreatedAt));
    }

    [HttpPost("seed-worldcup")]
    public async Task<ActionResult> SeedWorldCup()
    {
        var result = await WorldCupSeeder.SeedAsync(_db);
        return Ok(result);
    }

    [HttpPost("recalculate")]
    public async Task<ActionResult> Recalculate()
    {
        await _scoringService.RecalculateAllAsync();
        return Ok(new { message = "Pontuação recalculada com sucesso." });
    }

    [HttpPut("matches/{id:guid}/result")]
    public async Task<ActionResult> UpdateMatchResult(Guid id, UpdateMatchResultRequest request)
    {
        if (request.HomeScore < 0 || request.AwayScore < 0)
        {
            return BadRequest(new { message = "O placar não pode ser negativo." });
        }

        var match = await _db.Matches.FirstOrDefaultAsync(x => x.Id == id);
        if (match is null)
        {
            return NotFound(new { message = "Jogo não encontrado." });
        }

        match.HomeScore = request.HomeScore;
        match.AwayScore = request.AwayScore;
        match.WinnerTeamId = request.WinnerTeamId;
        match.Status = "finished";

        await _db.SaveChangesAsync();
        await _scoringService.RecalculateMatchAsync(id);

        return Ok(new { message = "Resultado atualizado e pontuação recalculada." });
    }

    private static string NormalizeName(string? value) => (value ?? string.Empty).Trim();

    private static string? NormalizeEmail(string? value)
    {
        var email = value?.Trim().ToLowerInvariant();
        return string.IsNullOrWhiteSpace(email) ? null : email;
    }
}
