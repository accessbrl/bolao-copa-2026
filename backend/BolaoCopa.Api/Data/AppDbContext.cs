using BolaoCopa.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BolaoCopa.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<ScoringRule> ScoringRules => Set<ScoringRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(x => x.Name).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(180).IsRequired(false);
            entity.Property(x => x.Role).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("teams");
            entity.HasIndex(x => x.FifaCode).IsUnique();
            entity.Property(x => x.FifaCode).HasMaxLength(10).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.GroupCode).HasMaxLength(5).IsRequired();
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.ToTable("matches");
            entity.HasIndex(x => x.MatchNumber).IsUnique();
            entity.Property(x => x.Stage).HasMaxLength(40).IsRequired();
            entity.Property(x => x.GroupCode).HasMaxLength(5);
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Venue).HasMaxLength(150);
            entity.Property(x => x.City).HasMaxLength(100);
            entity.Property(x => x.HomePlaceholder).HasMaxLength(80);
            entity.Property(x => x.AwayPlaceholder).HasMaxLength(80);

            entity.HasOne(x => x.HomeTeam)
                .WithMany(x => x.HomeMatches)
                .HasForeignKey(x => x.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.AwayTeam)
                .WithMany(x => x.AwayMatches)
                .HasForeignKey(x => x.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.WinnerTeam)
                .WithMany()
                .HasForeignKey(x => x.WinnerTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.ToTable("predictions");
            entity.HasIndex(x => new { x.UserId, x.MatchId }).IsUnique();

            entity.HasOne(x => x.User)
                .WithMany(x => x.Predictions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Match)
                .WithMany(x => x.Predictions)
                .HasForeignKey(x => x.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.PredictedWinnerTeam)
                .WithMany()
                .HasForeignKey(x => x.PredictedWinnerTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ScoringRule>(entity =>
        {
            entity.ToTable("scoring_rules");
        });
    }
}
