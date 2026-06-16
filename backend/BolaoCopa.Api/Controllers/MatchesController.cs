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
public class MatchesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ScoringService _scoringService;

    public MatchesController(AppDbContext db, ScoringService scoringService)
    {
        _db = db;
        _scoringService = scoringService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetAll()
    {
        var rule = await _scoringService.GetRuleAsync();
        var matches = await _db.Matches
            .Include(x => x.HomeTeam)
            .Include(x => x.AwayTeam)
            .OrderBy(x => x.KickoffAtUtc)
            .ThenBy(x => x.MatchNumber)
            .ToListAsync();

        return Ok(matches.Select(x => ToDto(x, rule)));
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetUpcoming([FromQuery] int take = 8)
    {
        var rule = await _scoringService.GetRuleAsync();
        var now = DateTime.UtcNow.AddHours(-4);

        var matches = await _db.Matches
            .Include(x => x.HomeTeam)
            .Include(x => x.AwayTeam)
            .Where(x => x.KickoffAtUtc >= now && x.Status != "finished")
            .OrderBy(x => x.KickoffAtUtc)
            .Take(Math.Clamp(take, 1, 20))
            .ToListAsync();

        return Ok(matches.Select(x => ToDto(x, rule)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MatchDto>> GetById(Guid id)
    {
        var rule = await _scoringService.GetRuleAsync();
        var match = await _db.Matches
            .Include(x => x.HomeTeam)
            .Include(x => x.AwayTeam)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (match is null)
        {
            return NotFound();
        }

        return Ok(ToDto(match, rule));
    }

    private MatchDto ToDto(Match match, ScoringRule rule)
    {
        return new MatchDto(
            match.Id,
            match.MatchNumber,
            match.Stage,
            match.GroupCode,
            match.HomeTeamId,
            match.HomeTeam?.Name ?? match.HomePlaceholder ?? "A definir",
            match.HomeTeam?.FifaCode,
            match.AwayTeamId,
            match.AwayTeam?.Name ?? match.AwayPlaceholder ?? "A definir",
            match.AwayTeam?.FifaCode,
            match.KickoffAtUtc,
            match.Venue,
            match.City,
            match.Status,
            match.HomeScore,
            match.AwayScore,
            match.WinnerTeamId,
            _scoringService.IsMatchLocked(match, rule)
        );
    }
}
