

// Models/Database/AuditRecord.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Chapter_12.Models.Database
{
    [Table("AuditRecords")]
    public class AuditRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string AuditId { get; set; }

        [Required]
        [StringLength(200)]
        public string DatasetName { get; set; }

        public DateTime AuditDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public double OverallBiasScore { get; set; }

        [StringLength(20)]
        public string RiskLevel { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string FullResponse { get; set; }

        public int SuggestionCount { get; set; }

        public DateTime CreatedAt { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<BiasFindingEntity> BiasFindings { get; set; }
        public virtual ICollection<SuggestionEntity> Suggestions { get; set; }
    }
}