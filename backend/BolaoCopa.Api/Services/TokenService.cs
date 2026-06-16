using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BolaoCopa.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace BolaoCopa.Api.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Generate(AppUser user)
    {
        var secret = _configuration["JWT_SECRET"] ?? _configuration["Jwt:Secret"]!;
        var issuer = _configuration["Jwt:Issuer"] ?? "BolaoCopa2026";
        var audience = _configuration["Jwt:Audience"] ?? "BolaoCopa2026";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
