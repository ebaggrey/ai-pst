
using System.ComponentModel.DataAnnotations;
namespace Chapter_2.Models
{
    
    public class FirstImpressionRequest
    {
        [Required]
        [Url]
        public string RepositoryUrl { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string[] FocusAreas { get; set; } = Array.Empty<string>();

        public string? Branch { get; set; }

        [Range(1, 1000)]
        public int MaxRecommendations { get; set; } = 50;

        public AnalysisMetadata Metadata { get; set; } = new();
    }

    public class AnalysisMetadata
    {
        public string? Context { get; set; }
        public string[]? ExcludePatterns { get; set; }
        public string[]? IncludePatterns { get; set; }
        public bool IncludeTestAnalysis { get; set; } = true;
        public bool IncludeSecurityAnalysis { get; set; } = false;
        public int MaxRecommendations { get; set; } = 20;
    }



    public class QuestionGenerationRequest
    {
        [Required]
        public string TargetRole { get; set; } = "Senior QA Engineer";

        [Required]
        [Range(1, 20)]
        public int Count { get; set; } = 10;

        public string Style { get; set; } = "technical"; // collaborative, technical, cultural
        public string Audience { get; set; } = "team-members";
        public string[] FocusAreas { get; set; } = Array.Empty<string>();
        public int ExperienceLevel { get; set; } = 3; // 1-5 scale
        public string? Context { get; set; }
    }





}
