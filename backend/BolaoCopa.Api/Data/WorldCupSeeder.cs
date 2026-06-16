using System.Globalization;
using BolaoCopa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BolaoCopa.Api.Data;

public static class WorldCupSeeder
{
    public static async Task<object> SeedAsync(AppDbContext db)
    {
        var teamsAdded = 0;
        var matchesAdded = 0;

        var teams = new[]
        {
            new TeamSeed("MEX", "México", "A"),
            new TeamSeed("RSA", "África do Sul", "A"),
            new TeamSeed("KOR", "Coreia do Sul", "A"),
            new TeamSeed("CZE", "Tchéquia", "A"),
            new TeamSeed("CAN", "Canadá", "B"),
            new TeamSeed("BIH", "Bósnia e Herzegovina", "B"),
            new TeamSeed("QAT", "Catar", "B"),
            new TeamSeed("SUI", "Suíça", "B"),
            new TeamSeed("BRA", "Brasil", "C"),
            new TeamSeed("MAR", "Marrocos", "C"),
            new TeamSeed("HTI", "Haiti", "C"),
            new TeamSeed("SCO", "Escócia", "C"),
            new TeamSeed("USA", "Estados Unidos", "D"),
            new TeamSeed("PAR", "Paraguai", "D"),
            new TeamSeed("AUS", "Austrália", "D"),
            new TeamSeed("TUR", "Turquia", "D"),
            new TeamSeed("GER", "Alemanha", "E"),
            new TeamSeed("CUW", "Curaçao", "E"),
            new TeamSeed("CIV", "Costa do Marfim", "E"),
            new TeamSeed("ECU", "Equador", "E"),
            new TeamSeed("NED", "Países Baixos", "F"),
            new TeamSeed("JPN", "Japão", "F"),
            new TeamSeed("SWE", "Suécia", "F"),
            new TeamSeed("TUN", "Tunísia", "F"),
            new TeamSeed("BEL", "Bélgica", "G"),
            new TeamSeed("EGY", "Egito", "G"),
            new TeamSeed("IRI", "Irã", "G"),
            new TeamSeed("NZL", "Nova Zelândia", "G"),
            new TeamSeed("ESP", "Espanha", "H"),
            new TeamSeed("CPV", "Cabo Verde", "H"),
            new TeamSeed("KSA", "Arábia Saudita", "H"),
            new TeamSeed("URU", "Uruguai", "H"),
            new TeamSeed("FRA", "França", "I"),
            new TeamSeed("SEN", "Senegal", "I"),
            new TeamSeed("IRQ", "Iraque", "I"),
            new TeamSeed("NOR", "Noruega", "I"),
            new TeamSeed("ARG", "Argentina", "J"),
            new TeamSeed("DZA", "Argélia", "J"),
            new TeamSeed("AUT", "Áustria", "J"),
            new TeamSeed("JOR", "Jordânia", "J"),
            new TeamSeed("POR", "Portugal", "K"),
            new TeamSeed("COD", "República Democrática do Congo", "K"),
            new TeamSeed("UZB", "Uzbequistão", "K"),
            new TeamSeed("COL", "Colômbia", "K"),
            new TeamSeed("ENG", "Inglaterra", "L"),
            new TeamSeed("CRO", "Croácia", "L"),
            new TeamSeed("GHA", "Gana", "L"),
            new TeamSeed("PAN", "Panamá", "L")
        };

        foreach (var item in teams)
        {
            var team = await db.Teams.FirstOrDefaultAsync(x => x.FifaCode == item.Code);
            if (team is null)
            {
                db.Teams.Add(new Team
                {
                    FifaCode = item.Code,
                    Name = item.Name,
                    GroupCode = item.Group
                });
                teamsAdded++;
            }
            else
            {
                team.Name = item.Name;
                team.GroupCode = item.Group;
            }
        }

        await db.SaveChangesAsync();
        var teamMap = await db.Teams.ToDictionaryAsync(x => x.FifaCode, x => x);

        var matches = new[]
        {
            new MatchSeed(1, "GROUP", "A", "MEX", "RSA", null, null, "2026-06-11T19:00:00Z"),
            new MatchSeed(2, "GROUP", "A", "KOR", "CZE", null, null, "2026-06-12T02:00:00Z"),
            new MatchSeed(3, "GROUP", "B", "CAN", "BIH", null, null, "2026-06-12T19:00:00Z"),
            new MatchSeed(4, "GROUP", "D", "USA", "PAR", null, null, "2026-06-13T01:00:00Z"),
            new MatchSeed(5, "GROUP", "B", "QAT", "SUI", null, null, "2026-06-13T19:00:00Z"),
            new MatchSeed(6, "GROUP", "C", "BRA", "MAR", null, null, "2026-06-13T22:00:00Z"),
            new MatchSeed(7, "GROUP", "C", "HTI", "SCO", null, null, "2026-06-14T01:00:00Z"),
            new MatchSeed(8, "GROUP", "D", "AUS", "TUR", null, null, "2026-06-14T04:00:00Z"),
            new MatchSeed(9, "GROUP", "E", "GER", "CUW", null, null, "2026-06-14T17:00:00Z"),
            new MatchSeed(10, "GROUP", "F", "NED", "JPN", null, null, "2026-06-14T20:00:00Z"),
            new MatchSeed(11, "GROUP", "E", "CIV", "ECU", null, null, "2026-06-14T23:00:00Z"),
            new MatchSeed(12, "GROUP", "F", "SWE", "TUN", null, null, "2026-06-15T02:00:00Z"),
            new MatchSeed(13, "GROUP", "H", "ESP", "CPV", null, null, "2026-06-15T16:00:00Z"),
            new MatchSeed(14, "GROUP", "G", "BEL", "EGY", null, null, "2026-06-15T19:00:00Z"),
            new MatchSeed(15, "GROUP", "H", "KSA", "URU", null, null, "2026-06-15T22:00:00Z"),
            new MatchSeed(16, "GROUP", "G", "IRI", "NZL", null, null, "2026-06-16T01:00:00Z"),
            new MatchSeed(17, "GROUP", "I", "FRA", "SEN", null, null, "2026-06-16T19:00:00Z"),
            new MatchSeed(18, "GROUP", "I", "IRQ", "NOR", null, null, "2026-06-16T22:00:00Z"),
            new MatchSeed(19, "GROUP", "J", "ARG", "DZA", null, null, "2026-06-17T01:00:00Z"),
            new MatchSeed(20, "GROUP", "J", "AUT", "JOR", null, null, "2026-06-17T04:00:00Z"),
            new MatchSeed(21, "GROUP", "K", "POR", "COD", null, null, "2026-06-17T17:00:00Z"),
            new MatchSeed(22, "GROUP", "L", "ENG", "CRO", null, null, "2026-06-17T20:00:00Z"),
            new MatchSeed(23, "GROUP", "L", "GHA", "PAN", null, null, "2026-06-17T23:00:00Z"),
            new MatchSeed(24, "GROUP", "K", "UZB", "COL", null, null, "2026-06-18T02:00:00Z"),
            new MatchSeed(25, "GROUP", "A", "CZE", "RSA", null, null, "2026-06-18T16:00:00Z"),
            new MatchSeed(26, "GROUP", "B", "SUI", "BIH", null, null, "2026-06-18T19:00:00Z"),
            new MatchSeed(27, "GROUP", "B", "CAN", "QAT", null, null, "2026-06-18T22:00:00Z"),
            new MatchSeed(28, "GROUP", "A", "MEX", "KOR", null, null, "2026-06-19T01:00:00Z"),
            new MatchSeed(29, "GROUP", "D", "USA", "AUS", null, null, "2026-06-19T19:00:00Z"),
            new MatchSeed(30, "GROUP", "C", "SCO", "MAR", null, null, "2026-06-19T22:00:00Z"),
            new MatchSeed(31, "GROUP", "C", "BRA", "HTI", null, null, "2026-06-20T00:30:00Z"),
            new MatchSeed(32, "GROUP", "D", "TUR", "PAR", null, null, "2026-06-20T03:00:00Z"),
            new MatchSeed(33, "GROUP", "F", "NED", "SWE", null, null, "2026-06-20T17:00:00Z"),
            new MatchSeed(34, "GROUP", "E", "GER", "CIV", null, null, "2026-06-20T20:00:00Z"),
            new MatchSeed(35, "GROUP", "E", "ECU", "CUW", null, null, "2026-06-21T00:00:00Z"),
            new MatchSeed(36, "GROUP", "F", "TUN", "JPN", null, null, "2026-06-21T04:00:00Z"),
            new MatchSeed(37, "GROUP", "H", "ESP", "KSA", null, null, "2026-06-21T16:00:00Z"),
            new MatchSeed(38, "GROUP", "G", "BEL", "IRI", null, null, "2026-06-21T19:00:00Z"),
            new MatchSeed(39, "GROUP", "H", "URU", "CPV", null, null, "2026-06-21T22:00:00Z"),
            new MatchSeed(40, "GROUP", "G", "NZL", "EGY", null, null, "2026-06-22T01:00:00Z"),
            new MatchSeed(41, "GROUP", "J", "ARG", "AUT", null, null, "2026-06-22T17:00:00Z"),
            new MatchSeed(42, "GROUP", "I", "FRA", "IRQ", null, null, "2026-06-22T21:00:00Z"),
            new MatchSeed(43, "GROUP", "I", "NOR", "SEN", null, null, "2026-06-23T00:00:00Z"),
            new MatchSeed(44, "GROUP", "J", "JOR", "DZA", null, null, "2026-06-23T03:00:00Z"),
            new MatchSeed(45, "GROUP", "K", "POR", "UZB", null, null, "2026-06-23T17:00:00Z"),
            new MatchSeed(46, "GROUP", "L", "ENG", "GHA", null, null, "2026-06-23T20:00:00Z"),
            new MatchSeed(47, "GROUP", "L", "PAN", "CRO", null, null, "2026-06-23T23:00:00Z"),
            new MatchSeed(48, "GROUP", "K", "COL", "COD", null, null, "2026-06-24T02:00:00Z"),
            new MatchSeed(49, "GROUP", "B", "SUI", "CAN", null, null, "2026-06-24T19:00:00Z"),
            new MatchSeed(50, "GROUP", "B", "BIH", "QAT", null, null, "2026-06-24T19:00:00Z"),
            new MatchSeed(51, "GROUP", "C", "SCO", "BRA", null, null, "2026-06-24T22:00:00Z"),
            new MatchSeed(52, "GROUP", "C", "MAR", "HTI", null, null, "2026-06-24T22:00:00Z"),
            new MatchSeed(53, "GROUP", "A", "CZE", "MEX", null, null, "2026-06-25T01:00:00Z"),
            new MatchSeed(54, "GROUP", "A", "RSA", "KOR", null, null, "2026-06-25T01:00:00Z"),
            new MatchSeed(55, "GROUP", "E", "ECU", "GER", null, null, "2026-06-25T20:00:00Z"),
            new MatchSeed(56, "GROUP", "E", "CUW", "CIV", null, null, "2026-06-25T20:00:00Z"),
            new MatchSeed(57, "GROUP", "F", "TUN", "NED", null, null, "2026-06-25T23:00:00Z"),
            new MatchSeed(58, "GROUP", "F", "JPN", "SWE", null, null, "2026-06-25T23:00:00Z"),
            new MatchSeed(59, "GROUP", "D", "TUR", "USA", null, null, "2026-06-26T02:00:00Z"),
            new MatchSeed(60, "GROUP", "D", "PAR", "AUS", null, null, "2026-06-26T02:00:00Z"),
            new MatchSeed(61, "GROUP", "I", "NOR", "FRA", null, null, "2026-06-26T19:00:00Z"),
            new MatchSeed(62, "GROUP", "I", "SEN", "IRQ", null, null, "2026-06-26T19:00:00Z"),
            new MatchSeed(63, "GROUP", "H", "URU", "ESP", null, null, "2026-06-27T00:00:00Z"),
            new MatchSeed(64, "GROUP", "H", "CPV", "KSA", null, null, "2026-06-27T00:00:00Z"),
            new MatchSeed(65, "GROUP", "G", "NZL", "BEL", null, null, "2026-06-27T03:00:00Z"),
            new MatchSeed(66, "GROUP", "G", "EGY", "IRI", null, null, "2026-06-27T03:00:00Z"),
            new MatchSeed(67, "GROUP", "L", "PAN", "ENG", null, null, "2026-06-27T21:00:00Z"),
            new MatchSeed(68, "GROUP", "L", "CRO", "GHA", null, null, "2026-06-27T21:00:00Z"),
            new MatchSeed(69, "GROUP", "K", "COL", "POR", null, null, "2026-06-27T23:30:00Z"),
            new MatchSeed(70, "GROUP", "K", "COD", "UZB", null, null, "2026-06-27T23:30:00Z"),
            new MatchSeed(71, "GROUP", "J", "JOR", "ARG", null, null, "2026-06-28T02:00:00Z"),
            new MatchSeed(72, "GROUP", "J", "DZA", "AUT", null, null, "2026-06-28T02:00:00Z"),
            new MatchSeed(73, "ROUND_OF_32", null, null, null, "Classificado 1", "Classificado 2", "2026-06-28T17:00:00Z"),
            new MatchSeed(74, "ROUND_OF_32", null, null, null, "Classificado 3", "Classificado 4", "2026-06-28T21:00:00Z"),
            new MatchSeed(75, "ROUND_OF_32", null, null, null, "Classificado 5", "Classificado 6", "2026-06-29T17:00:00Z"),
            new MatchSeed(76, "ROUND_OF_32", null, null, null, "Classificado 7", "Classificado 8", "2026-06-29T21:00:00Z"),
            new MatchSeed(77, "ROUND_OF_32", null, null, null, "Classificado 9", "Classificado 10", "2026-06-30T17:00:00Z"),
            new MatchSeed(78, "ROUND_OF_32", null, null, null, "Classificado 11", "Classificado 12", "2026-06-30T21:00:00Z"),
            new MatchSeed(79, "ROUND_OF_32", null, null, null, "Classificado 13", "Classificado 14", "2026-07-01T17:00:00Z"),
            new MatchSeed(80, "ROUND_OF_32", null, null, null, "Classificado 15", "Classificado 16", "2026-07-01T21:00:00Z"),
            new MatchSeed(81, "ROUND_OF_32", null, null, null, "Classificado 17", "Classificado 18", "2026-07-02T17:00:00Z"),
            new MatchSeed(82, "ROUND_OF_32", null, null, null, "Classificado 19", "Classificado 20", "2026-07-02T21:00:00Z"),
            new MatchSeed(83, "ROUND_OF_32", null, null, null, "Classificado 21", "Classificado 22", "2026-07-03T17:00:00Z"),
            new MatchSeed(84, "ROUND_OF_32", null, null, null, "Classificado 23", "Classificado 24", "2026-07-03T21:00:00Z"),
            new MatchSeed(85, "ROUND_OF_32", null, null, null, "Classificado 25", "Classificado 26", "2026-07-03T23:00:00Z"),
            new MatchSeed(86, "ROUND_OF_32", null, null, null, "Classificado 27", "Classificado 28", "2026-07-04T01:00:00Z"),
            new MatchSeed(87, "ROUND_OF_32", null, null, null, "Classificado 29", "Classificado 30", "2026-07-04T03:00:00Z"),
            new MatchSeed(88, "ROUND_OF_32", null, null, null, "Classificado 31", "Classificado 32", "2026-07-04T05:00:00Z"),
            new MatchSeed(89, "ROUND_OF_16", null, null, null, "Vencedor jogo 73", "Vencedor jogo 74", "2026-07-04T17:00:00Z"),
            new MatchSeed(90, "ROUND_OF_16", null, null, null, "Vencedor jogo 75", "Vencedor jogo 76", "2026-07-04T21:00:00Z"),
            new MatchSeed(91, "ROUND_OF_16", null, null, null, "Vencedor jogo 77", "Vencedor jogo 78", "2026-07-05T17:00:00Z"),
            new MatchSeed(92, "ROUND_OF_16", null, null, null, "Vencedor jogo 79", "Vencedor jogo 80", "2026-07-05T21:00:00Z"),
            new MatchSeed(93, "ROUND_OF_16", null, null, null, "Vencedor jogo 81", "Vencedor jogo 82", "2026-07-06T17:00:00Z"),
            new MatchSeed(94, "ROUND_OF_16", null, null, null, "Vencedor jogo 83", "Vencedor jogo 84", "2026-07-06T21:00:00Z"),
            new MatchSeed(95, "ROUND_OF_16", null, null, null, "Vencedor jogo 85", "Vencedor jogo 86", "2026-07-07T17:00:00Z"),
            new MatchSeed(96, "ROUND_OF_16", null, null, null, "Vencedor jogo 87", "Vencedor jogo 88", "2026-07-07T21:00:00Z"),
            new MatchSeed(97, "QUARTER_FINAL", null, null, null, "Vencedor jogo 89", "Vencedor jogo 90", "2026-07-09T20:00:00Z"),
            new MatchSeed(98, "QUARTER_FINAL", null, null, null, "Vencedor jogo 91", "Vencedor jogo 92", "2026-07-10T19:00:00Z"),
            new MatchSeed(99, "QUARTER_FINAL", null, null, null, "Vencedor jogo 93", "Vencedor jogo 94", "2026-07-11T21:00:00Z"),
            new MatchSeed(100, "QUARTER_FINAL", null, null, null, "Vencedor jogo 95", "Vencedor jogo 96", "2026-07-12T01:00:00Z"),
            new MatchSeed(101, "SEMI_FINAL", null, null, null, "Vencedor jogo 97", "Vencedor jogo 98", "2026-07-14T20:00:00Z"),
            new MatchSeed(102, "SEMI_FINAL", null, null, null, "Vencedor jogo 99", "Vencedor jogo 100", "2026-07-15T20:00:00Z"),
            new MatchSeed(103, "THIRD_PLACE", null, null, null, "Perdedor jogo 101", "Perdedor jogo 102", "2026-07-18T21:00:00Z"),
            new MatchSeed(104, "FINAL", null, null, null, "Vencedor jogo 101", "Vencedor jogo 102", "2026-07-19T19:00:00Z")
        };

        foreach (var item in matches)
        {
            var kickoff = DateTime.Parse(item.KickoffAtUtc, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            Guid? homeTeamId = item.HomeCode is not null && teamMap.TryGetValue(item.HomeCode, out var home) ? home.Id : (Guid?)null;
            Guid? awayTeamId = item.AwayCode is not null && teamMap.TryGetValue(item.AwayCode, out var away) ? away.Id : (Guid?)null;

            var match = await db.Matches.FirstOrDefaultAsync(x => x.MatchNumber == item.MatchNumber);
            if (match is null)
            {
                match = new Match
                {
                    MatchNumber = item.MatchNumber,
                    Venue = "A definir",
                    City = "A definir",
                    Status = "scheduled"
                };
                db.Matches.Add(match);
                matchesAdded++;
            }

            // Atualiza os dados base da tabela sem apagar placares já informados pelo administrador.
            match.Stage = item.Stage;
            match.GroupCode = item.GroupCode;
            match.HomeTeamId = homeTeamId;
            match.AwayTeamId = awayTeamId;
            match.HomePlaceholder = item.HomePlaceholder;
            match.AwayPlaceholder = item.AwayPlaceholder;
            match.KickoffAtUtc = kickoff;
            match.Venue ??= "A definir";
            match.City ??= "A definir";
        }

        await db.SaveChangesAsync();

        return new
        {
            message = "Seed da Copa 2026 executado com sucesso.",
            teamsAdded,
            matchesAdded,
            totalTeams = await db.Teams.CountAsync(),
            totalMatches = await db.Matches.CountAsync()
        };
    }

    private record TeamSeed(string Code, string Name, string Group);

    private record MatchSeed(
        int MatchNumber,
        string Stage,
        string? GroupCode,
        string? HomeCode,
        string? AwayCode,
        string? HomePlaceholder,
        string? AwayPlaceholder,
        string KickoffAtUtc
    );
}
