namespace Chapter_4.Settings
{
    // Models/Configuration/AITestingConfiguration.cs
    public class AITestingConfiguration
    {
        public int MaxDimensionsForThorough { get; set; } = 10;
        public int MaxPromptVariations { get; set; } = 50;
        public decimal MinSensitivityThreshold { get; set; } = 0.5m;
        public decimal MaxSensitivityThreshold { get; set; } = 0.95m;
        public int MinFactsForHallucinationTest { get; set; } = 5;
        public int MinBaselineResults { get; set; } = 10;
        public int MinDataPointsForDrift { get; set; } = 50;
        public int DefaultTestRuns { get; set; } = 10;
        public int RequestTimeoutSeconds { get; set; } = 30;
        public bool EnableCaching { get; set; } = true;
        public int CacheDurationMinutes { get; set; } = 60;
        public string[] SupportedProviders { get; set; } = { "openai", "anthropic", "google", "azure" };
    }
}
