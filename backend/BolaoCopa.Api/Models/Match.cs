namespace BolaoCopa.Api.Models;

public class Match
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int MatchNumber { get; set; }
    public string Stage { get; set; } = "GROUP";
    public string? GroupCode { get; set; }

    public Guid? HomeTeamId { get; set; }
    public Team? HomeTeam { get; set; }

    public Guid? AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }

    public string? HomePlaceholder { get; set; }
    public string? AwayPlaceholder { get; set; }

    public DateTime KickoffAtUtc { get; set; }
    public string? Venue { get; set; }
    public string? City { get; set; }

    public string Status { get; set; } = "scheduled";
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }

    public Guid? WinnerTeamId { get; set; }
    public Team? WinnerTeam { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
}
