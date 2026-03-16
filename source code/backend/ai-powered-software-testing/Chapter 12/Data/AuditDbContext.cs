
// Data/AuditDbContext.cs
using Chapter_12.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Chapter_12.Data
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuditRecord> AuditRecords { get; set; }
        public DbSet<BiasFindingEntity> BiasFindings { get; set; }
        public DbSet<SuggestionEntity> Suggestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure AuditRecord
            modelBuilder.Entity<AuditRecord>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AuditId)
                    .IsUnique();

                entity.HasIndex(e => e.DatasetName);
                entity.HasIndex(e => e.AuditDate);

                entity.Property(e => e.AuditId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DatasetName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RiskLevel)
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100);

                // Configure one-to-many relationship with BiasFindingEntity
                entity.HasMany(e => e.BiasFindings)
                    .WithOne(e => e.AuditRecord)
                    .HasForeignKey(e => e.AuditRecordId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure one-to-many relationship with SuggestionEntity
                entity.HasMany(e => e.Suggestions)
                    .WithOne(e => e.AuditRecord)
                    .HasForeignKey(e => e.AuditRecordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure BiasFindingEntity
            modelBuilder.Entity<BiasFindingEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AuditRecordId);
                entity.HasIndex(e => e.BiasType);

                entity.Property(e => e.FieldName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.BiasType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Examples)
                    .HasColumnType("nvarchar(max)");

                // Configure relationship back to AuditRecord
                entity.HasOne(e => e.AuditRecord)
                    .WithMany(e => e.BiasFindings)
                    .HasForeignKey(e => e.AuditRecordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SuggestionEntity
            modelBuilder.Entity<SuggestionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AuditRecordId);
                entity.HasIndex(e => e.FieldName);

                entity.Property(e => e.OriginalValue)
                    .HasMaxLength(500);

                entity.Property(e => e.SuggestedValue)
                    .HasMaxLength(500);

                entity.Property(e => e.FieldName)
                    .HasMaxLength(100);

                entity.Property(e => e.Rationale)
                    .HasMaxLength(1000);

                // Configure relationship back to AuditRecord
                entity.HasOne(e => e.AuditRecord)
                    .WithMany(e => e.Suggestions)
                    .HasForeignKey(e => e.AuditRecordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}