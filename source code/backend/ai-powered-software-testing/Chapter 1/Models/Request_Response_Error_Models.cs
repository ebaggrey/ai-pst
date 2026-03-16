using Chapter_1.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chapter_1.Models
{

  

    public class LandscapeTestRequest
    {
        [Required(ErrorMessage = "Application profile is required")]
        public ApplicationProfile ApplicationProfile { get; set; } = new ApplicationProfile();

        [Required(ErrorMessage = "Testing focus areas are required")]
        [MinLength(1, ErrorMessage = "Tell us what to focus on testing!")]
        public string[] TestingFocus { get; set; } = Array.Empty<string>();

        [ValidateRiskAssessment(ErrorMessage = "Risk assessment contains invalid values")]
        public RiskAssessment RiskAssessment { get; set; } = new RiskAssessment();

        [RegularExpression(@"^\d+\.\d+$", ErrorMessage = "Prompt version must be in format X.X")]
        public string PromptVersion { get; set; } = "1.0";

        [MinLength(1, ErrorMessage = "At least one artifact type is required")]
        public string[] RequestedArtifacts { get; set; } =
            new[] { "testScenarios", "automationScripts" };

        // Additional properties for enhanced functionality
        public Dictionary<string, string> CustomParameters { get; set; } = new();
        public bool IncludeDetailedAnalysis { get; set; } = true;
        public string AnalysisDepth { get; set; } = "comprehensive"; // quick, standard, comprehensive

        [Range(1, 100)]
        public int MaxRecommendationsPerArea { get; set; } = 10;

        [JsonIgnore]
        public bool IsValid => ApplicationProfile != null &&
                               TestingFocus != null &&
                               TestingFocus.Length > 0;
    }

    // Models/Landscape/LandscapeError.cs
    public class LandscapeError
    {
        public string ErrorCode { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string[] RecoverySteps { get; set; } = Array.Empty<string>();

        public string FallbackSuggestion { get; set; } = string.Empty;

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Dictionary<string, object> Context { get; set; } = new();

        // Additional properties for enhanced error handling
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        public string RequestId { get; set; } = string.Empty;

        public string HelpUrl { get; set; } = "https://docs.testing-landscape.com/errors";

        public string Severity { get; set; } = "error"; // error, warning, info

        public Dictionary<string, string> Metadata { get; set; } = new();

        // Helper properties
        [JsonIgnore]
        public bool IsRecoverable => RecoverySteps?.Length > 0;

        [JsonIgnore]
        public bool HasFallback => !string.IsNullOrEmpty(FallbackSuggestion);

        // Factory methods for common errors
        public static LandscapeError ValidationError(Dictionary<string, string[]> errors)
        {
            return new LandscapeError
            {
                ErrorCode = "VALIDATION_FAILED",
                Message = "The request contains validation errors",
                RecoverySteps = new[] { "Review the error details below", "Correct the invalid fields" },
                Context = new Dictionary<string, object> { ["validationErrors"] = errors }
            };
        }

        public static LandscapeError ServiceUnavailable(string serviceName, string reason = "")
        {
            return new LandscapeError
            {
                ErrorCode = $"SERVICE_UNAVAILABLE_{serviceName.ToUpper()}",
                Message = $"The {serviceName} service is currently unavailable" +
                         (string.IsNullOrEmpty(reason) ? "" : $": {reason}"),
                RecoverySteps = new[] { "Try again in a few minutes", "Check service status dashboard" },
                FallbackSuggestion = "Use offline analysis mode with basic recommendations"
            };
        }
    }



    // Models/Landscape/TestLandscapeResponse.cs
    public class TestLandscapeResponse
    {
        public string AnalysisId { get; set; } = Guid.NewGuid().ToString();

        public TestScenario[] HighPriorityScenarios { get; set; } = Array.Empty<TestScenario>();

        public AutomationBlueprint[] RecommendedAutomation { get; set; } = Array.Empty<AutomationBlueprint>();

        public RiskHotspot[] IdentifiedRisks { get; set; } = Array.Empty<RiskHotspot>();

        public FlakyTestPrediction[] FlakyPredictions { get; set; } = Array.Empty<FlakyTestPrediction>();

        public MonitoringStrategy MonitoringSuggestions { get; set; } = new MonitoringStrategy();

        public string Summary { get; set; } = string.Empty;

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public TimeSpan EstimatedEffort { get; set; }

        // Additional properties for enhanced response
        public Dictionary<string, decimal> ConfidenceScores { get; set; } = new()
    {
        { "scenarios", 0.85m },
        { "automation", 0.75m },
        { "risks", 0.90m },
        { "flaky_predictions", 0.70m }
    };

        public string[] UsedLLMProviders { get; set; } = Array.Empty<string>();

        public Dictionary<string, object> AnalysisMetadata { get; set; } = new();

        public string NextSteps { get; set; } = string.Empty;

        public ResourceRecommendation[] ResourceRecommendations { get; set; } = Array.Empty<ResourceRecommendation>();

        public CostEstimate CostEstimate { get; set; } = new CostEstimate();

        // Helper methods - FIXED: Changed return type to DateTime
        [JsonIgnore]
        public DateTime EstimatedCompletionTime => GeneratedAt.Add(EstimatedEffort);

        [JsonIgnore]
        public int TotalRecommendations =>
            HighPriorityScenarios.Length +
            RecommendedAutomation.Length +
            IdentifiedRisks.Length +
            FlakyPredictions.Length;

        [JsonIgnore]
        public bool HasCriticalRisks =>
            IdentifiedRisks.Any(r => r.RiskLevel == "critical");

        // Alternative property if you want the time until completion
        [JsonIgnore]
        public TimeSpan TimeUntilCompletion =>
            EstimatedCompletionTime - DateTime.UtcNow;

        // Property to check if analysis is still valid (not expired)
        [JsonIgnore]
        public bool IsAnalysisValid =>
            GeneratedAt.AddDays(30) > DateTime.UtcNow; // Valid for 30 days

        // Property to get formatted completion time
        [JsonIgnore]
        public string FormattedCompletionTime =>
            EstimatedCompletionTime.ToString("yyyy-MM-dd HH:mm");

        // Property to get effort in hours
        [JsonIgnore]
        public double EffortInHours => EstimatedEffort.TotalHours;

        // Property to categorize effort level
        [JsonIgnore]
        public string EffortLevel
        {
            get
            {
                var hours = EffortInHours;
                return hours switch
                {
                    < 8 => "Small (1 day)",
                    < 40 => "Medium (1 week)",
                    < 160 => "Large (1 month)",
                    _ => "Very Large (> 1 month)"
                };
            }
        }

        // Factory method for successful response
        public static TestLandscapeResponse CreateSuccess(
            TestScenario[] scenarios,
            AutomationBlueprint[] automation,
            RiskHotspot[] risks,
            FlakyTestPrediction[] predictions,
            string summary)
        {
            return new TestLandscapeResponse
            {
                AnalysisId = Guid.NewGuid().ToString(),
                HighPriorityScenarios = scenarios,
                RecommendedAutomation = automation,
                IdentifiedRisks = risks,
                FlakyPredictions = predictions,
                Summary = summary,
                GeneratedAt = DateTime.UtcNow,
                EstimatedEffort = CalculateEffort(scenarios, automation),
                MonitoringSuggestions = new MonitoringStrategy()
            };
        }

        private static TimeSpan CalculateEffort(TestScenario[] scenarios, AutomationBlueprint[] automation)
        {
            var scenarioHours = scenarios.Length * 2; // 2 hours per scenario
            var automationHours = automation.Length * 8; // 8 hours per automation script
            return TimeSpan.FromHours(scenarioHours + automationHours);
        }

        // Method to get a quick summary of the response
        public object GetQuickSummary()
        {
            return new
            {
                AnalysisId,
                TotalScenarios = HighPriorityScenarios.Length,
                TotalAutomationScripts = RecommendedAutomation.Length,
                TotalRisks = IdentifiedRisks.Length,
                EstimatedCompletion = FormattedCompletionTime,
                EffortLevel
            };
        }
    }



    // Supporting models for enhanced response
    public class ResourceRecommendation
    {
        public string Type { get; set; } = string.Empty; // tool, framework, library, service
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
        public string[] UseCases { get; set; } = Array.Empty<string>();
        public CostRange Cost { get; set; } = new CostRange();
        public string[] Alternatives { get; set; } = Array.Empty<string>();
    }

    public class CostRange
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string Currency { get; set; } = "USD";
        public string Period { get; set; } = "month"; // month, year, one-time
    }

    public class CostEstimate
    {
        public decimal AutomationDevelopment { get; set; }
        public decimal ManualTesting { get; set; }
        public decimal ToolLicensing { get; set; }
        public decimal Infrastructure { get; set; }
        public decimal Total => AutomationDevelopment + ManualTesting + ToolLicensing + Infrastructure;
        public string Currency { get; set; } = "USD";
        public string Timeframe { get; set; } = "3 months"; // 1 month, 3 months, 1 year
    }



    // Custom JSON converter for DateTime
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
    }


}




