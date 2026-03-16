using Chapter_7.Data.Entities;
using Microsoft.EntityFrameworkCore;


namespace Chapter_7.Data.Context
{
    
    public class PipelineDbContext : DbContext
    {
        public PipelineDbContext(DbContextOptions<PipelineDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<PipelineStage> PipelineStages { get; set; }
        public DbSet<PipelineRun> PipelineRuns { get; set; }
        public DbSet<PipelineFailure> PipelineFailures { get; set; }
        public DbSet<OptimizationOpportunity> OptimizationOpportunities { get; set; }
        public DbSet<AdaptationEvent> AdaptationEvents { get; set; }
        public DbSet<PredictionModel> PredictionModels { get; set; }
        public DbSet<PredictionResult> PredictionResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Pipeline configuration
            modelBuilder.Entity<Pipeline>(entity =>
            {
                entity.ToTable("Pipelines");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PipelineId).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Language);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // PipelineStage configuration
            modelBuilder.Entity<PipelineStage>(entity =>
            {
                entity.ToTable("PipelineStages");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.PipelineId, e.StageId }).IsUnique();

                entity.HasOne(e => e.Pipeline)
                    .WithMany(e => e.Stages)
                    .HasForeignKey(e => e.PipelineId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PipelineRun configuration
            modelBuilder.Entity<PipelineRun>(entity =>
            {
                entity.ToTable("PipelineRuns");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.RunId).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.TriggeredAt);

                entity.HasOne(e => e.Pipeline)
                    .WithMany(e => e.Runs)
                    .HasForeignKey(e => e.PipelineId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PipelineFailure configuration
            modelBuilder.Entity<PipelineFailure>(entity =>
            {
                entity.ToTable("PipelineFailures");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.FailureType);

                entity.HasOne(e => e.Run)
                    .WithMany(e => e.Failures)
                    .HasForeignKey(e => e.RunId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OptimizationOpportunity configuration
            modelBuilder.Entity<OptimizationOpportunity>(entity =>
            {
                entity.ToTable("OptimizationOpportunities");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OpportunityId).IsUnique();

                entity.HasOne(e => e.Pipeline)
                    .WithMany(e => e.OptimizationOpportunities)
                    .HasForeignKey(e => e.PipelineId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // AdaptationEvent configuration
            modelBuilder.Entity<AdaptationEvent>(entity =>
            {
                entity.ToTable("AdaptationEvents");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.AdaptationId).IsUnique();

                entity.HasOne(e => e.Pipeline)
                    .WithMany(e => e.AdaptationEvents)
                    .HasForeignKey(e => e.PipelineId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PredictionModel configuration
            modelBuilder.Entity<PredictionModel>(entity =>
            {
                entity.ToTable("PredictionModels");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ModelId, e.Version }).IsUnique();
            });

            // PredictionResult configuration
            modelBuilder.Entity<PredictionResult>(entity =>
            {
                entity.ToTable("PredictionResults");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PredictionId).IsUnique();

                entity.HasOne(e => e.Pipeline)
                    .WithMany(e => e.PredictionResults)
                    .HasForeignKey(e => e.PipelineId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Model)
                    .WithMany(e => e.PredictionResults)
                    .HasForeignKey(e => e.ModelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
