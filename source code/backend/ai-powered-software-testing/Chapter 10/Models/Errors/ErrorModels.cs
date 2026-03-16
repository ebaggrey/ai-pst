
// Models/Errors/MetricErrorResponse.cs
namespace Chapter_10.Models.Errors
{
    public class MetricErrorResponse
    {
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public string[] RecoverySteps { get; set; }
        public string FallbackSuggestion { get; set; }
        public MetricDiagnosticData DiagnosticData { get; set; }
    }

    public class MetricDiagnosticData
    {
        public string[] AmbiguousObjectives { get; set; }
        public string[] ClarificationQuestions { get; set; }
        public InconsistencyDetails InconsistencyDetails { get; set; }
        public DataQualityIssue[] DataQualityIssues { get; set; }
        public string[] PatternDetectionChallenges { get; set; }
        public string[] DataRequirements { get; set; }
        public string[] InsightGenerationChallenges { get; set; }
        public string[] MetricLimitations { get; set; }
        public string[] ConflictingGoals { get; set; }
        public TradeOffAnalysis TradeOffAnalysis { get; set; }
    }

    public class InconsistencyDetails
    {
        public string[] InconsistentMetrics { get; set; }
        public string[] TimePeriodIssues { get; set; }
        public string[] UnitMismatches { get; set; }
    }

    public class DataQualityIssue
    {
        public string MetricId { get; set; }
        public string Issue { get; set; }
        public string Severity { get; set; }
    }

    public class TradeOffAnalysis
    {
        public string[] Tradeoffs { get; set; }
        public Dictionary<string, double> Impacts { get; set; }
    }
}