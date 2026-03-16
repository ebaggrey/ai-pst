
// Data/SpectrumDbContext.cs
using Chapter_11.Models.Analysis;
using Chapter_11.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Monitor = Chapter_11.Models.Responses.Monitor;

namespace Chapter_11.Data
{
    public class SpectrumDbContext : DbContext
    {
        public SpectrumDbContext(DbContextOptions<SpectrumDbContext> options)
            : base(options)
        {
        }

        public DbSet<AcceptanceCriteria> AcceptanceCriteria { get; set; }
        public DbSet<TestScenario> TestScenarios { get; set; }
        public DbSet<TestDataRequirement> TestDataRequirements { get; set; }
        public DbSet<RiskAssessment> RiskAssessments { get; set; }
        public DbSet<Monitor> Monitors { get; set; }
        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<Orchestration> Orchestrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configure UserBehaviorPattern
            modelBuilder.Entity<UserBehaviorPattern>(entity =>
            {
                entity.HasKey(e => e.Id);

                // For TimeSpan, store as TimeSpan in SQL Server (supported in EF Core)
                entity.Property(e => e.AverageDuration)
                    .HasConversion<TimeSpan>();
            });

            // Configure JourneyStep
            modelBuilder.Entity<JourneyStep>(entity =>
            {
                entity.Property(e => e.TypicalDuration)
                    .HasConversion<TimeSpan>();
            });

            // Configure AcceptanceCriteria
            modelBuilder.Entity<AcceptanceCriteria>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<AcceptanceCriteria>()
                .Property(a => a.Criterion)
                .IsRequired();

            // Configure TestScenario
            modelBuilder.Entity<TestScenario>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TestScenario>()
                .Property(t => t.Steps)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            // Configure TestDataRequirement
            modelBuilder.Entity<TestDataRequirement>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<TestDataRequirement>()
                .Property(t => t.SampleData)
                .HasColumnType("jsonb");

            // Configure RiskAssessment
            modelBuilder.Entity<RiskAssessment>()
                .HasKey(r => r.Id);

            // Configure Monitor
            modelBuilder.Entity<Monitor>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Monitor>()
                .Property(m => m.Configuration)
                .HasColumnType("jsonb");

            // Configure Pipeline
            modelBuilder.Entity<Pipeline>()
                .HasKey(p => p.PipelineId);

            modelBuilder.Entity<Pipeline>()
                .OwnsMany(p => p.Stages);

            // Configure Orchestration
            modelBuilder.Entity<Orchestration>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<Orchestration>()
                .OwnsMany(o => o.ExecutionPlans);
        }
    }
}
