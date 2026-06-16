using System.Security.Claims;
using BolaoCopa.Api.Data;
using BolaoCopa.Api.DTOs;
using BolaoCopa.Api.Models;
using BolaoCopa.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BolaoCopa.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PredictionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ScoringService _scoringService;

    public PredictionsController(AppDbContext db, ScoringService scoringService)
    {
        _db = db;
        _scoringService = scoringService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<PredictionDto>>> GetMine()
    {
        var userId = GetUserId();
        var predictions = await _db.Predictions
            .Include(x => x.Match).ThenInclude(x => x.HomeTeam)
            .Include(x => x.Match).ThenInclude(x => x.AwayTeam)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Match.KickoffAtUtc)
            .ToListAsync();

        return Ok(predictions.Select(ToDto));
    }

    [HttpPost]
    public async Task<ActionResult<PredictionDto>> Save(SavePredictionRequest request)
    {
        if (request.PredictedHomeScore < 0 || request.PredictedAwayScore < 0)
        {
            return BadRequest(new { message = "O placar não pode ser negativo." });
        }

        if (request.PredictedHomeScore > 30 || request.PredictedAwayScore > 30)
        {
            return BadRequest(new { message = "Placar muito alto. Verifique o palpite." });
        }

        var userId = GetUserId();
        var rule = await _scoringService.GetRuleAsync();
        var match = await _db.Matches
            .Include(x => x.HomeTeam)
            .Include(x => x.AwayTeam)
            .FirstOrDefaultAsync(x => x.Id == request.MatchId);

        if (match is null)
        {
            return NotFound(new { message = "Jogo não encontrado." });
        }

        if (_scoringService.IsMatchLocked(match, rule))
        {
            return BadRequest(new { message = "Palpite bloqueado para este jogo." });
        }

        var prediction = await _db.Predictions
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MatchId == request.MatchId);

        if (prediction is null)
        {
            prediction = new Prediction
            {
                UserId = userId,
                MatchId = request.MatchId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Predictions.Add(prediction);
        }

        prediction.PredictedHomeScore = request.PredictedHomeScore;
        prediction.PredictedAwayScore = request.PredictedAwayScore;
        prediction.PredictedWinnerTeamId = request.PredictedWinnerTeamId;
        prediction.UpdatedAt = DateTime.UtcNow;

        _scoringService.CalculatePrediction(prediction, match, rule);
        await _db.SaveChangesAsync();

        prediction.Match = match;
        return Ok(ToDto(prediction));
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException("Usuário inválido.");
        }
        return userId;
    }

    private static PredictionDto ToDto(Prediction prediction)
    {
        var match = prediction.Match;
        return new PredictionDto(
            prediction.Id,
            prediction.MatchId,
            match.MatchNumber,
            match.HomeTeam?.Name ?? match.HomePlaceholder ?? "A definir",
            match.AwayTeam?.Name ?? match.AwayPlaceholder ?? "A definir",
            prediction.PredictedHomeScore,
            prediction.PredictedAwayScore,
            prediction.PredictedWinnerTeamId,
            prediction.Points,
            prediction.IsExactScore,
            prediction.IsOutcomeCorrect,
            prediction.IsGoalDiffCorrect,
            prediction.UpdatedAt
        );
    }
}
