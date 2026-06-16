namespace BolaoCopa.Api.Models;

public class ScoringRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ExactScorePoints { get; set; } = 10;
    public int ResultAndGoalDiffPoints { get; set; } = 6;
    public int ResultOnlyPoints { get; set; } = 3;
    public int ChampionPoints { get; set; } = 20;
    public int LockMinutesBeforeMatch { get; set; } = 5;
}
