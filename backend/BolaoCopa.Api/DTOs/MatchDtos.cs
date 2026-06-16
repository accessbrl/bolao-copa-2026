namespace BolaoCopa.Api.DTOs;

public record MatchDto(
    Guid Id,
    int MatchNumber,
    string Stage,
    string? GroupCode,
    Guid? HomeTeamId,
    string HomeName,
    string? HomeCode,
    Guid? AwayTeamId,
    string AwayName,
    string? AwayCode,
    DateTime KickoffAtUtc,
    string? Venue,
    string? City,
    string Status,
    int? HomeScore,
    int? AwayScore,
    Guid? WinnerTeamId,
    bool IsLocked
);

public record UpdateMatchResultRequest(
    int HomeScore,
    int AwayScore,
    Guid? WinnerTeamId
);
