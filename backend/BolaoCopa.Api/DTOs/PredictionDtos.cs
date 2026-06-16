namespace BolaoCopa.Api.DTOs;

public record SavePredictionRequest(
    Guid MatchId,
    int PredictedHomeScore,
    int PredictedAwayScore,
    Guid? PredictedWinnerTeamId
);

public record PredictionDto(
    Guid Id,
    Guid MatchId,
    int MatchNumber,
    string HomeName,
    string AwayName,
    int PredictedHomeScore,
    int PredictedAwayScore,
    Guid? PredictedWinnerTeamId,
    int Points,
    bool IsExactScore,
    bool IsOutcomeCorrect,
    bool IsGoalDiffCorrect,
    DateTime UpdatedAt
);
