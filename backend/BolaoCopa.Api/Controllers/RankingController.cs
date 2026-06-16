using BolaoCopa.Api.Data;
using BolaoCopa.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolaoCopa.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RankingController : ControllerBase
{
    private readonly AppDbContext _db;

    public RankingController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RankingDto>>> Get()
    {
        var users = await _db.Users
            .Include(x => x.Predictions)
            .Where(x => x.IsActive)
            .ToListAsync();

        var ranking = users
            .Select(x => new
            {
                x.Id,
                x.Name,
                TotalPoints = x.Predictions.Sum(p => p.Points),
                ExactScores = x.Predictions.Count(p => p.IsExactScore),
                OutcomeHits = x.Predictions.Count(p => p.IsOutcomeCorrect),
                PredictionsCount = x.Predictions.Count
            })
            .OrderByDescending(x => x.TotalPoints)
            .ThenByDescending(x => x.ExactScores)
            .ThenByDescending(x => x.OutcomeHits)
            .ThenBy(x => x.Name)
            .Select((x, index) => new RankingDto(
                index + 1,
                x.Id,
                x.Name,
                x.TotalPoints,
                x.ExactScores,
                x.OutcomeHits,
                x.PredictionsCount
            ))
            .ToList();

        return Ok(ranking);
    }
}
