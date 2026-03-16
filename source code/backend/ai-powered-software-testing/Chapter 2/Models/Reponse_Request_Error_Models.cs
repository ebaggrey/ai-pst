namespace Chapter_2.Models
{
    public class CodebaseAnalysisResponse
    {
        public string AnalysisId { get; set; } = Guid.NewGuid().ToString();

        [Range(1, 10)]
        public decimal MaturityScore { get; set; }

        public string HighestRiskArea { get; set; } = string.Empty;

        public string[] QuickWins { get; set; } = Array.Empty<string>();

        public string[] TechnicalDebtAreas { get; set; } = Array.Empty<string>();

        public TestCoverageBreakdown TestCoverage { get; set; } = new();

        public RecommendationTimeline[] RecommendationTimeline { get; set; } = Array.Empty<RecommendationTimeline>();

        public string[] NextSteps { get; set; } = Array.Empty<string>();

        [Range(0, 1)]
        public double ConfidenceScore { get; set; }

        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class OnboardingErrorResponse
    {
        public string ErrorId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Phase { get; set; } = string.Empty; // discovery, integration, execution

        [Required]
        public string ErrorType { get; set; } = string.Empty; // connectivity, analysis, recommendation

        public string Message { get; set; } = string.Empty;

        public string RecoveryAction { get; set; } = string.Empty;

        public string SuggestedFallback { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        public Dictionary<string, object> Context { get; set; } = new();
    }

   

    // Required attributes (simplified versions - in real project, use System.ComponentModel.DataAnnotations)
    public class RangeAttribute : Attribute
    {
        public object Minimum { get; }
        public object Maximum { get; }

        public RangeAttribute(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public RangeAttribute(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }

    public class RequiredAttribute : Attribute
    {
        public bool AllowEmptyStrings { get; set; }
    }
}
