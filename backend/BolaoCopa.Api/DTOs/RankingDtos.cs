namespace BolaoCopa.Api.DTOs;

public record RankingDto(
    int Position,
    Guid UserId,
    string Name,
    int TotalPoints,
    int ExactScores,
    int OutcomeHits,
    int PredictionsCount
);
