namespace Chapter_5.Configuration
{
    // Models/Configuration/TDDConfiguration.cs
    public class TDDConfiguration
    {
        public int MaxTestComplexity { get; set; } = 15;
        public string DefaultImplementationStrategy { get; set; } = "simplest-first";
        public string[] AllowedTddStyles { get; set; } = new[] { "classic", "outside-in", "inside-out" };
        public string DefaultTimeHorizon { get; set; } = "quarterly";
        public int MinTestCountForRefactoring { get; set; } = 3;
        public bool EnableFuturePredictions { get; set; } = true;
        public ComplexityThresholds ComplexityThresholds { get; set; } = new ComplexityThresholds();
        public QualityMetrics QualityMetrics { get; set; } = new QualityMetrics();
        public RefactoringSettings Refactoring { get; set; } = new RefactoringSettings();

        // Development settings
        public bool EnableDetailedLogging { get; set; } = true;
        public bool EnableVirtualTestExecution { get; set; } = true;
        public bool SimulateAsyncDelays { get; set; } = true;
        public int DelayMilliseconds { get; set; } = 100;
        public double SuccessRate { get; set; } = 0.95;
        public int MaxGeneratedTests { get; set; } = 10;
        public int MaxImplementationOptions { get; set; } = 5;
        public double DefaultConfidenceThreshold { get; set; } = 0.7;
    }

    public class ComplexityThresholds
    {
        public int Low { get; set; } = 5;
        public int Medium { get; set; } = 10;
        public int High { get; set; } = 20;
    }

    public class QualityMetrics
    {
        public double MinConfidenceScore { get; set; } = 0.6;
        public int MaxCyclomaticComplexity { get; set; } = 15;
        public double TargetMaintainabilityIndex { get; set; } = 0.7;
    }

    public class RefactoringSettings
    {
        public int MaxStepsWithoutCheckpoint { get; set; } = 5;
        public bool EnableSafetyChecks { get; set; } = true;
        public string DefaultRollbackStrategy { get; set; } = "last-successful";
        public double SuccessRate { get; set; } = 0.95;
    }
}
