namespace BolaoCopa.Api.Models;

public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FifaCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GroupCode { get; set; } = string.Empty;

    public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
    public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
}
