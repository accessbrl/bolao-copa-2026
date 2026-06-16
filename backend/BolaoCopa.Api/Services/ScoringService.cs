using BolaoCopa.Api.Data;
using BolaoCopa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BolaoCopa.Api.Services;

public class ScoringService
{
    private readonly AppDbContext _db;

    public ScoringService(AppDbContext db)
    {
        _db = db;
    }

    public bool IsMatchLocked(Match match, ScoringRule rule)
    {
        if (match.Status.Equals("finished", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return DateTime.UtcNow >= match.KickoffAtUtc.AddMinutes(-rule.LockMinutesBeforeMatch);
    }

    public async Task<int> CalculatePredictionAsync(Prediction prediction)
    {
        var match = await _db.Matches.FirstAsync(x => x.Id == prediction.MatchId);
        var rule = await GetRuleAsync();
        CalculatePrediction(prediction, match, rule);
        return prediction.Points;
    }

    public void CalculatePrediction(Prediction prediction, Match match, ScoringRule rule)
    {
        prediction.Points = 0;
        prediction.IsExactScore = false;
        prediction.IsOutcomeCorrect = false;
        prediction.IsGoalDiffCorrect = false;

        if (!match.Status.Equals("finished", StringComparison.OrdinalIgnoreCase)
            || match.HomeScore is null
            || match.AwayScore is null)
        {
            return;
        }

        var actualHome = match.HomeScore.Value;
        var actualAway = match.AwayScore.Value;
        var predictedHome = prediction.PredictedHomeScore;
        var predictedAway = prediction.PredictedAwayScore;

        prediction.IsExactScore = predictedHome == actualHome && predictedAway == actualAway;

        var actualOutcome = Math.Sign(actualHome - actualAway);
        var predictedOutcome = Math.Sign(predictedHome - predictedAway);
        prediction.IsOutcomeCorrect = actualOutcome == predictedOutcome;

        var actualDiff = actualHome - actualAway;
        var predictedDiff = predictedHome - predictedAway;
        prediction.IsGoalDiffCorrect = actualDiff == predictedDiff;

        if (prediction.IsExactScore)
        {
            prediction.Points = rule.ExactScorePoints;
        }
        else if (prediction.IsOutcomeCorrect && prediction.IsGoalDiffCorrect)
        {
            prediction.Points = rule.ResultAndGoalDiffPoints;
        }
        else if (prediction.IsOutcomeCorrect)
        {
            prediction.Points = rule.ResultOnlyPoints;
        }
    }

    public async Task RecalculateMatchAsync(Guid matchId)
    {
        var rule = await GetRuleAsync();
        var match = await _db.Matches.FirstAsync(x => x.Id == matchId);
        var predictions = await _db.Predictions.Where(x => x.MatchId == matchId).ToListAsync();

        foreach (var prediction in predictions)
        {
            CalculatePrediction(prediction, match, rule);
            prediction.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task RecalculateAllAsync()
    {
        var rule = await GetRuleAsync();
        var matches = await _db.Matches.ToDictionaryAsync(x => x.Id);
        var predictions = await _db.Predictions.ToListAsync();

        foreach (var prediction in predictions)
        {
            if (matches.TryGetValue(prediction.MatchId, out var match))
            {
                CalculatePrediction(prediction, match, rule);
                prediction.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<ScoringRule> GetRuleAsync()
    {
        var rule = await _db.ScoringRules.FirstOrDefaultAsync();
        if (rule is not null)
        {
            return rule;
        }

        rule = new ScoringRule();
        _db.ScoringRules.Add(rule);
        await _db.SaveChangesAsync();
        return rule;
    }
}
