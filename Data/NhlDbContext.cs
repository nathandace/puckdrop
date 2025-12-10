using Microsoft.EntityFrameworkCore;
using PuckDrop.Models.Domain;

namespace PuckDrop.Data;

/// <summary>
/// Entity Framework database context for the NHL monitoring application.
/// </summary>
public class NhlDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NhlDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public NhlDbContext(DbContextOptions<NhlDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the favorite team records.
    /// </summary>
    public DbSet<FavoriteTeam> FavoriteTeams { get; set; }

    /// <summary>
    /// Gets or sets the webhook rule records.
    /// </summary>
    public DbSet<WebhookRule> WebhookRules { get; set; }

    /// <summary>
    /// Gets or sets the processed event records for deduplication.
    /// </summary>
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

    /// <summary>
    /// Gets or sets the webhook log records for history tracking.
    /// </summary>
    public DbSet<WebhookLog> WebhookLogs { get; set; }

    /// <summary>
    /// Configures the entity models and relationships.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FavoriteTeam>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TeamAbbrev).IsRequired().HasMaxLength(10);
            entity.Property(e => e.TeamName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.PrimaryColor).HasMaxLength(10);
            entity.Property(e => e.SecondaryColor).HasMaxLength(10);
        });

        modelBuilder.Entity<WebhookRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TeamAbbrev).IsRequired().HasMaxLength(10);
            entity.Property(e => e.TargetUrl).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.EventType).HasConversion<int>();
            entity.Property(e => e.PayloadFormat).HasConversion<int>();
        });

        modelBuilder.Entity<ProcessedEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EventType).HasConversion<int>();
            entity.HasIndex(e => new { e.GameId, e.EventId }).IsUnique();
            // Index for cleanup queries by date
            entity.HasIndex(e => e.ProcessedAt);
        });

        modelBuilder.Entity<WebhookRule>(entity =>
        {
            // Index for querying enabled rules by team and event type
            entity.HasIndex(e => new { e.IsEnabled, e.TeamAbbrev, e.EventType });
        });

        modelBuilder.Entity<WebhookLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).HasConversion<int>();
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.Property(e => e.EventDescription).HasMaxLength(500);

            // Foreign key to WebhookRule
            entity.HasOne(e => e.WebhookRule)
                .WithMany()
                .HasForeignKey(e => e.WebhookRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for querying by rule and cleanup by date
            entity.HasIndex(e => e.WebhookRuleId);
            entity.HasIndex(e => e.TriggeredAt);
        });
    }
}
