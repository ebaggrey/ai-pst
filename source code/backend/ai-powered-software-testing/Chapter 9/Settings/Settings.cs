
namespace Chapter_9.Settings
{
    public class LeanTestingSettings
    {
        public int MaxFeaturesPerBatch { get; set; }
        public double DefaultConfidenceTarget { get; set; }
        public double ParetoThreshold { get; set; }
        public double MinROIThreshold { get; set; }
        public int MaxTestCasesPerFeature { get; set; }
        public bool EnableCaching { get; set; }
        public int CacheDurationMinutes { get; set; }
        public FeatureFlags Features { get; set; }
        public ValidationSettings Validation { get; set; }
    }

    public class FeatureFlags
    {
        public bool PrioritizationEnabled { get; set; }
        public bool CoverageGenerationEnabled { get; set; }
        public bool AutomationDecisionEnabled { get; set; }
        public bool MaintenanceOptimizationEnabled { get; set; }
        public bool ROIMeasurementEnabled { get; set; }
    }

    public class ValidationSettings
    {
        public bool StrictMode { get; set; }
        public bool ValidateConstraints { get; set; }
        public bool EnforceLeanPrinciples { get; set; }
    }

    public class LLMSettings
    {
        public string BaseUrl { get; set; }
        public string ModelName { get; set; }
        public string ApiKey { get; set; }
        public int TimeoutSeconds { get; set; }
        public int MaxTokens { get; set; }
        public double Temperature { get; set; }
        public bool Enabled { get; set; }
        public string Provider { get; set; }
        public Dictionary<string, string> Endpoints { get; set; }
    }
}
