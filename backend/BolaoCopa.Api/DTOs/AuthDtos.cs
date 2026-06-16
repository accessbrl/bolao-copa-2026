namespace BolaoCopa.Api.DTOs;

public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
}

public record UserDto(
    Guid Id,
    string Name,
    string? Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);

public record LoginResponse(
    string Token,
    UserDto User
);
