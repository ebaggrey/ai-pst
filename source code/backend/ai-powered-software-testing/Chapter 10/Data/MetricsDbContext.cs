
// Data/MetricsDbContext.cs
using Chapter_10.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Chapter_10.Data
{
    public class MetricsDbContext : DbContext
    {
        public MetricsDbContext(DbContextOptions<MetricsDbContext> options)
            : base(options)
        {
        }

        public DbSet<MetricDesignEntity> MetricDesigns { get; set; }
        public DbSet<HealthScoreEntity> HealthScores { get; set; }
        public DbSet<PredictionEntity> Predictions { get; set; }
        public DbSet<InsightEntity> Insights { get; set; }
        public DbSet<OptimizationEntity> Optimizations { get; set; }
        public DbSet<MetricDefinitionEntity> MetricDefinitions { get; set; }
        public DbSet<HistoricalDataEntity> HistoricalData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // MetricDesign Entity
            modelBuilder.Entity<MetricDesignEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DesignId).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.DesignId).IsUnique();
            });

            // HealthScore Entity
            modelBuilder.Entity<HealthScoreEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HealthScoreId).IsRequired();
                entity.Property(e => e.CalculatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.HealthScoreId).IsUnique();
            });

            // MetricDefinition Entity
            modelBuilder.Entity<MetricDefinitionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MetricId).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.HasIndex(e => e.MetricId).IsUnique();
            });

            // HistoricalData Entity
            modelBuilder.Entity<HistoricalDataEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MetricId).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.HasIndex(e => new { e.MetricId, e.Timestamp });
            });

            // Configure JSON serialization
            modelBuilder.Entity<MetricDesignEntity>()
                .Property(e => e.Metrics)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<DesignedMetric[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<DesignedMetric>());

            modelBuilder.Entity<MetricDesignEntity>()
                .Property(e => e.BusinessObjectives)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<string>());
        }
    }

    // Entity Models
    public class MetricDesignEntity
    {
        public int Id { get; set; }
        public string DesignId { get; set; } = string.Empty;
        public string[] BusinessObjectives { get; set; } = Array.Empty<string>();
        public DesignedMetric[] Metrics { get; set; } = Array.Empty<DesignedMetric>();
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class HealthScoreEntity
    {
        public int Id { get; set; }
        public string HealthScoreId { get; set; } = string.Empty;
        public double OverallScore { get; set; }
        public double Confidence { get; set; }
        public string ComponentScores { get; set; } = string.Empty; // JSON
        public string MetricValues { get; set; } = string.Empty; // JSON
        public DateTime CalculatedAt { get; set; }
        public string CalculatedBy { get; set; } = string.Empty;
    }

    public class PredictionEntity
    {
        public int Id { get; set; }
        public string PredictionId { get; set; } = string.Empty;
        public string MetricId { get; set; } = string.Empty;
        public int PredictionHorizon { get; set; }
        public string PredictedValues { get; set; } = string.Empty; // JSON
        public double Confidence { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ValidUntil { get; set; }
    }

    public class InsightEntity
    {
        public int Id { get; set; }
        public string InsightId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double ActionabilityScore { get; set; }
        public string AffectedMetrics { get; set; } = string.Empty; // JSON
        public string RecommendedActions { get; set; } = string.Empty; // JSON
        public DateTime GeneratedAt { get; set; }
        public bool IsImplemented { get; set; }
    }

    public class OptimizationEntity
    {
        public int Id { get; set; }
        public string OptimizationId { get; set; } = string.Empty;
        public string Recommendations { get; set; } = string.Empty; // JSON
        public double EstimatedSavings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ImplementedAt { get; set; }
        public string Status { get; set; } = "pending";
    }

    public class MetricDefinitionEntity
    {
        public int Id { get; set; }
        public string MetricId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string CollectionMethod { get; set; } = string.Empty;
        public double CollectionCost { get; set; }
        public double BusinessValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastCollected { get; set; }
    }

    public class HistoricalDataEntity
    {
        public int Id { get; set; }
        public string MetricId { get; set; } = string.Empty;
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string Tags { get; set; } = string.Empty; // JSON
        public string Source { get; set; } = string.Empty;
    }
}
