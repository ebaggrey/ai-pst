
// Models/Database/BiasFindingEntity.cs
using Chapter_12.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chapter_12.Models.Database
{
    [Table("BiasFindings")]
    public class BiasFindingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AuditRecordId { get; set; }

        [ForeignKey("AuditRecordId")]
        public virtual AuditRecord AuditRecord { get; set; }

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; }

        [Required]
        [StringLength(50)]
        public string BiasType { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public double SeverityScore { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Examples { get; set; }
    }
}

// Models/Database/SuggestionEntity.cs
namespace Chapter_12.Models.Database
{
    [Table("Suggestions")]
    public class SuggestionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AuditRecordId { get; set; }

        [ForeignKey("AuditRecordId")]
        public virtual AuditRecord AuditRecord { get; set; }

        [StringLength(500)]
        public string OriginalValue { get; set; }

        [StringLength(500)]
        public string SuggestedValue { get; set; }

        [StringLength(100)]
        public string FieldName { get; set; }

        [StringLength(1000)]
        public string Rationale { get; set; }

        public int ConfidenceScore { get; set; }
    }
}