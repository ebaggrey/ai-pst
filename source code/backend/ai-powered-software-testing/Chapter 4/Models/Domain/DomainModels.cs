namespace Chapter_4.Models.Domain
{
    // Models/Requests/AIAssessmentRequest.cs
    public class AIAssessmentRequest
    {
        public string Provider { get; set; } = string.Empty;
        public string RigorLevel { get; set; } = "standard";
        public string[] Dimensions { get; set; } = Array.Empty<string>();
        public string ModelName { get; set; } = string.Empty;
        public int MaxTokens { get; set; } = 1000;
        public bool IncludeBenchmarks { get; set; } = true;
    }

    // Models/Responses/AICapabilityReport.cs
    public class AICapabilityReport
    {
        public string Provider { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public decimal OverallScore { get; set; }
        public Dictionary<string, decimal> DimensionScores { get; set; } = new();
        public List<CapabilityMetric> Metrics { get; set; } = new();
        public List<Recommendation> Recommendations { get; set; } = new();
        public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    }

    // Models/Responses/RobustnessTestReport.cs
    public class RobustnessTestReport
    {
        public string BasePrompt { get; set; } = string.Empty;
        public int VariationCount { get; set; }
        public int RunCount { get; set; }
        public decimal AverageConsistencyScore { get; set; }
        public List<VariationResult> VariationResults { get; set; } = new();
        public VarianceAnalysis VarianceAnalysis { get; set; } = new();
        public List<Antipattern> Antipatterns { get; set; } = new();
        public List<OptimizationSuggestion> OptimizationSuggestions { get; set; } = new();
    }

    // Models/Responses/BiasDetectionReport.cs
    public class BiasDetectionReport
    {
        public List<BiasFinding> Findings { get; set; } = new();
        public decimal OverallBiasScore { get; set; }
        public Dictionary<string, decimal> ContextBiasScores { get; set; } = new();
        public List<MitigationStrategy> MitigationStrategies { get; set; } = new();
        public MonitoringPlan LongTermMonitoringPlan { get; set; } = new();
        public bool StatisticalSignificanceValidated { get; set; }
    }

    // Models/Responses/HallucinationDetectionReport.cs
    public class HallucinationDetectionReport
    {
        public string Provider { get; set; } = string.Empty;
        public List<HallucinationFinding> Hallucinations { get; set; } = new();
        public decimal HallucinationRate { get; set; }
        public List<ConfidenceAdjustment> ConfidenceAdjustments { get; set; } = new();
        public List<VerificationRule> VerificationRules { get; set; } = new();
        public int TotalTests { get; set; }
        public int HallucinationCount { get; set; }

        public List<Recommendation> Recommendations { get; set; } = new();
    }

    // Models/Responses/DriftDetectionReport.cs
    public class DriftDetectionReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DriftSignificance { get; set; }
        public Dictionary<string, MetricDrift> MetricDrifts { get; set; } = new();
        public List<Alert> Alerts { get; set; } = new();
        public List<RecommendedAction> RecommendedActions { get; set; } = new();
        public BaselineComparison BaselineComparison { get; set; } = new();
    }

    // Models/Requests/RobustnessTestRequest.cs
    public class RobustnessTestRequest
    {
        public string BasePrompt { get; set; } = string.Empty;
        public string[] Variations { get; set; } = Array.Empty<string>();
        public int NumberOfRuns { get; set; } = 10;
        public string Provider { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
    }

    // Models/Requests/BiasDetectionRequest.cs
    public class BiasDetectionRequest
    {
        public string[] ContextAreas { get; set; } = Array.Empty<string>();
        public string[] DetectionMethods { get; set; } = Array.Empty<string>();
        public decimal SensitivityThreshold { get; set; } = 0.75m;
        public bool RequireStatisticalSignificance { get; set; } = false;
        public Dictionary<string, string> DemographicData { get; set; } = new();
    }

    // Models/Requests/HallucinationTestRequest.cs
    public class HallucinationTestRequest
    {
        public string Provider { get; set; } = string.Empty;
        public string[] KnownFacts { get; set; } = Array.Empty<string>();
        public int MaxAllowedHallucinations { get; set; } = 3;
        public string[] VerificationSources { get; set; } = Array.Empty<string>();
        public int TestIterations { get; set; } = 20;
    }

    // Models/Requests/DriftDetectionRequest.cs
    public class DriftDetectionRequest
    {
        public BaselineData Baseline { get; set; } = new();
        public string Timeframe { get; set; } = "30d";
        public string[] MetricsToMonitor { get; set; } = Array.Empty<string>();
        public decimal DriftThreshold { get; set; } = 0.15m;
        public int MinimumDataPoints { get; set; } = 100;
    }

    // Models/Supporting/DiagnosticInfo.cs
    public class DiagnosticInfo
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string[] SuggestedInvestigation { get; set; } = Array.Empty<string>();
    }

    // Models/Errors/AITestingError.cs
    public class AITestingError
    {
        public string TestType { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string FailureMode { get; set; } = string.Empty;
        public DiagnosticInfo DiagnosticInfo { get; set; } = new();
        public string FallbackAction { get; set; } = string.Empty;
        public TestMetadata Metadata { get; set; } = new();
    }

    // Models/Supporting/TestMetadata.cs
    public class TestMetadata
    {
        public string TestCaseId { get; set; } = Guid.NewGuid().ToString();
        public string InputPrompt { get; set; } = string.Empty;
        public string ExpectedBehavior { get; set; } = string.Empty;
    }

    // Models/Supporting/BaselineData.cs
    public class BaselineData
    {
        public TestResult[] TestResults { get; set; } = Array.Empty<TestResult>();
        public DateTime CollectedOn { get; set; }
        public string Environment { get; set; } = "production";
    }

    // Models/Supporting/TestResult.cs
    public class TestResult
    {
        public string TestId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Dictionary<string, decimal> Metrics { get; set; } = new();
        public string Status { get; set; } = "completed";
    }

    // Models/Supporting/CapabilityMetric.cs
    public class CapabilityMetric
    {
        public string Name { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public decimal Weight { get; set; } = 1.0m;
        public string Category { get; set; } = string.Empty;
    }

    // Models/Supporting/Recommendation.cs
    public class Recommendation
    {
        public string Area { get; set; } = string.Empty;
        public string Suggestion { get; set; } = string.Empty;
        public string Priority { get; set; } = "medium";
        public string Impact { get; set; } = string.Empty;
    }

    // Models/Supporting/VariationResult.cs
    public class VariationResult
    {
        public string Variation { get; set; } = string.Empty;
        public decimal ConsistencyScore { get; set; }
        public string[] Responses { get; set; } = Array.Empty<string>();
        public bool Passed { get; set; }
    }

    // Models/Supporting/VarianceAnalysis.cs
    public class VarianceAnalysis
    {
        public decimal OverallVariance { get; set; }
        public Dictionary<string, decimal> VariationVariances { get; set; } = new();
        public string[] HighVarianceVariations { get; set; } = Array.Empty<string>();
        public string[] StableVariations { get; set; } = Array.Empty<string>();
    }

    // Models/Supporting/Antipattern.cs
    public class Antipattern
    {
        public string Pattern { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "medium";
        public string Fix { get; set; } = string.Empty;
    }

    // Models/Supporting/OptimizationSuggestion.cs
    public class OptimizationSuggestion
    {
        public string Area { get; set; } = string.Empty;
        public string Suggestion { get; set; } = string.Empty;
        public decimal ExpectedImprovement { get; set; }
    }

    // Models/Supporting/BiasFinding.cs
    public class BiasFinding
    {
        public string Context { get; set; } = string.Empty;
        public string BiasType { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public string Evidence { get; set; } = string.Empty;
        public string Severity { get; set; } = "medium";
    }

    // Models/Supporting/MitigationStrategy.cs
    public class MitigationStrategy
    {
        public string FindingId { get; set; } = string.Empty;
        public string Strategy { get; set; } = string.Empty;
        public string Implementation { get; set; } = string.Empty;
        public string Timeline { get; set; } = "short-term";
    }

    // Models/Supporting/MonitoringPlan.cs
    public class MonitoringPlan
    {
        public string[] Metrics { get; set; } = Array.Empty<string>();
        public string Frequency { get; set; } = "weekly";
        public string[] Triggers { get; set; } = Array.Empty<string>();
        public string ReportingFormat { get; set; } = "dashboard";
    }

    // Models/Supporting/HallucinationFinding.cs
    public class HallucinationFinding
    {
        public string Fact { get; set; } = string.Empty;
        public string AIResponse { get; set; } = string.Empty;
        public string Severity { get; set; } = "medium";
        public string Category { get; set; } = string.Empty;
        public string Correction { get; set; } = string.Empty;
    }

    // Models/Supporting/ConfidenceAdjustment.cs
    public class ConfidenceAdjustment
    {
        public string Context { get; set; } = string.Empty;
        public decimal AdjustmentFactor { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    // Models/Supporting/VerificationRule.cs
    public class VerificationRule
    {
        public string Pattern { get; set; } = string.Empty;
        public string VerificationMethod { get; set; } = string.Empty;
        public string ConfidenceLevel { get; set; } = "high";
        public bool AutoVerify { get; set; }
    }

    // Models/Supporting/MetricDrift.cs
    public class MetricDrift
    {
        public string MetricName { get; set; } = string.Empty;
        public decimal BaselineValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal DriftAmount { get; set; }
        public string Direction { get; set; } = "stable";
        public bool Significant { get; set; }
    }

    // Models/Supporting/Alert.cs
    public class Alert
    {
        public string AlertId { get; set; } = Guid.NewGuid().ToString();
        public string Metric { get; set; } = string.Empty;
        public string Severity { get; set; } = "warning";
        public string Message { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    }

    // Models/Supporting/RecommendedAction.cs
    public class RecommendedAction
    {
        public string Action { get; set; } = string.Empty;
        public string Priority { get; set; } = "medium";
        public string Impact { get; set; } = string.Empty;
        public string Effort { get; set; } = "medium";
    }

    // Models/Supporting/BaselineComparison.cs
    public class BaselineComparison
    {
        public int DataPointsCompared { get; set; }
        public decimal SimilarityScore { get; set; }
        public DateTime ComparisonDate { get; set; } = DateTime.UtcNow;
    }
}
