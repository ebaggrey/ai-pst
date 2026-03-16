namespace Chapter_3.Settings
{
    // Settings/HumanLoopSettings.cs
    public class HumanLoopSettings
    {
        public ConfidenceThresholds ConfidenceThresholds { get; set; } = new();
        public ReviewGuidance ReviewGuidance { get; set; } = new();
        public CollaborationDefaults CollaborationDefaults { get; set; } = new();
        public LearningSettings LearningSettings { get; set; } = new();
    }

    public class ConfidenceThresholds
    {
        public double AutoApprove { get; set; } = 0.92;
        public double LightReview { get; set; } = 0.75;
        public double FullReview { get; set; } = 0.60;
        public double HumanRequired { get; set; } = 0.40;
    }

    public class ReviewGuidance
    {
        public string[] AlwaysReview { get; set; } = { "security-tests", "payment-flows", "data-migration" };
        public string[] QuickReview { get; set; } = { "ui-component-tests", "api-contract-tests", "utility-functions" };
        public string[] SampleReview { get; set; } = { "generated-bulk-tests", "similar-pattern-tests" };
    }

    public class CollaborationDefaults
    {
        public int MaxActiveSessionsPerUser { get; set; } = 5;
        public int SessionTimeoutMinutes { get; set; } = 120;
        public int AutoSaveIntervalSeconds { get; set; } = 30;
        public int MaxUndoSteps { get; set; } = 50;
        public int CollaboratorLimit { get; set; } = 5;
    }

    public class LearningSettings
    {
        public bool StoreAllJudgments { get; set; } = true;
        public int MinimumSamplesForRetraining { get; set; } = 100;
        public string RetrainingSchedule { get; set; } = "weekly";
        public double FeedbackImpactWeight { get; set; } = 0.7;
        public string HumanOverridePriority { get; set; } = "high";
    }

    // Settings/CollaborationSettings.cs
    public class CollaborationSettings
    {
        public ToolsSettings Tools { get; set; } = new();
    }

    public class ToolsSettings
    {
        public DiffViewSettings DiffView { get; set; } = new();
        public SuggestionsSettings Suggestions { get; set; } = new();
        public ChatSettings Chat { get; set; } = new();
    }

    public class DiffViewSettings
    {
        public bool Enabled { get; set; } = true;
        public bool SideBySide { get; set; } = true;
        public bool HighlightChanges { get; set; } = true;
        public bool WordLevelDiff { get; set; } = false;
    }

    public class SuggestionsSettings
    {
        public bool Enabled { get; set; } = true;
        public string Frequency { get; set; } = "on-edit";
        public int MaxSuggestions { get; set; } = 3;
        public bool AutoApply { get; set; } = false;
    }

    public class ChatSettings
    {
        public bool Enabled { get; set; } = true;
        public bool Persistent { get; set; } = true;
        public bool Threaded { get; set; } = true;
    }

    // Settings/LearningSettings.cs (already defined above, but separate for DI)
    public class LearningFromJudgmentsSettings
    {
        public bool EnableAutomaticLearning { get; set; } = true;
        public int BatchSizeForTraining { get; set; } = 50;
        public string[] PrioritizedCategories { get; set; } = { "security", "business_logic", "edge_cases" };
        public double MinimumConfidenceForLearning { get; set; } = 0.7;
    }
}
