namespace BolaoCopa.Api.Models;

public class Prediction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid MatchId { get; set; }
    public Match Match { get; set; } = null!;

    public int PredictedHomeScore { get; set; }
    public int PredictedAwayScore { get; set; }

    public Guid? PredictedWinnerTeamId { get; set; }
    public Team? PredictedWinnerTeam { get; set; }

    public int Points { get; set; }
    public bool IsExactScore { get; set; }
    public bool IsOutcomeCorrect { get; set; }
    public bool IsGoalDiffCorrect { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
