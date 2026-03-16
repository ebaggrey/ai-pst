namespace Chapter_6.Models.Requests
{
    // Models/Requests.cs
   
        public class BDCCoCreationRequest
        {
            public string Requirement { get; set; } = string.Empty;
            public StakeholderPerspective[] StakeholderPerspectives { get; set; } = Array.Empty<StakeholderPerspective>();
            public ConversationConstraints ConversationConstraints { get; set; } = new();
            public string[] DesiredOutcomes { get; set; } = Array.Empty<string>();
        }

        public class StakeholderPerspective
        {
            public string Role { get; set; } = string.Empty;
            public string[] Priorities { get; set; } = Array.Empty<string>();
            public string[] Concerns { get; set; } = Array.Empty<string>();
        }

        public class ConversationConstraints
        {
            public int MaxRounds { get; set; } = 5;
            public double ConsensusThreshold { get; set; } = 0.8;
            public string[] ForbiddenAssumptions { get; set; } = Array.Empty<string>();
        }

        public class AutomationRequest
        {
            public BDDScenario Scenario { get; set; } = new();
            public TechContext TechContext { get; set; } = new();
            public string TranslationStyle { get; set; } = "declarative";
            public QualityTargets QualityTargets { get; set; } = new();
        }

        public class BDDScenario
        {
            public string Title { get; set; } = string.Empty;
            public string[] Given { get; set; } = Array.Empty<string>();
            public string[] When { get; set; } = Array.Empty<string>();
            public string[] Then { get; set; } = Array.Empty<string>();
            public string[] Tags { get; set; } = Array.Empty<string>();
            public string Description { get; set; } = string.Empty;
            public string[] Examples { get; set; } = Array.Empty<string>();
        }

        public class TechContext
        {
            public string Stack { get; set; } = "dotnet";
            public string TestFramework { get; set; } = "xunit";
            public string[] Libraries { get; set; } = Array.Empty<string>();
            public string[] Constraints { get; set; } = Array.Empty<string>();
        }

        public class QualityTargets
        {
            public double MinCoverage { get; set; } = 0.8;
            public int MaxComplexity { get; set; } = 10;
            public int MaxDependencies { get; set; } = 5;
            public string[] ValidationRules { get; set; } = Array.Empty<string>();
        }

        public class EvolutionRequest
        {
            public BDDScenario[] ExistingScenarios { get; set; } = Array.Empty<BDDScenario>();
            public string NewInformation { get; set; } = string.Empty;
            public BreakingChange[] BreakingChanges { get; set; } = Array.Empty<BreakingChange>();
            public ValidationRule[] ValidationRules { get; set; } = Array.Empty<ValidationRule>();
            public string EvolutionStrategy { get; set; } = "preserve-intent";
        }

        public class BreakingChange
        {
            public string Type { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ImpactLevel { get; set; } = "medium";
            public string[] AffectedAreas { get; set; } = Array.Empty<string>();
        }

        public class ValidationRule
        {
            public string Name { get; set; } = string.Empty;
            public string Condition { get; set; } = string.Empty;
            public string ErrorMessage { get; set; } = string.Empty;
        }

        public class DriftDetectionRequest
        {
            public BDDScenario[] DocumentedScenarios { get; set; } = Array.Empty<BDDScenario>();
            public ImplementedBehavior[] ImplementedBehavior { get; set; } = Array.Empty<ImplementedBehavior>();
            public string[] DetectionMethods { get; set; } = Array.Empty<string>();
            public double Sensitivity { get; set; } = 0.7;
            public bool AutoSuggestFixes { get; set; } = false;
        }

        public class ImplementedBehavior
        {
            public string ScenarioId { get; set; } = string.Empty;
            public string[] Steps { get; set; } = Array.Empty<string>();
            public string[] Outcomes { get; set; } = Array.Empty<string>();
            public string[] EdgeCases { get; set; } = Array.Empty<string>();
            public DateTime LastUpdated { get; set; }
        }

        public class DocumentationRequest
        {
            public BDDScenario[] Scenarios { get; set; } = Array.Empty<BDDScenario>();
            public TestResult[] TestResults { get; set; } = Array.Empty<TestResult>();
            public Audience Audience { get; set; } = new();
            public string Format { get; set; } = "html";
            public string[] Include { get; set; } = Array.Empty<string>();
            public UpdateStrategy UpdateStrategy { get; set; } = new();
        }

        public class TestResult
        {
            public string ScenarioId { get; set; } = string.Empty;
            public bool Passed { get; set; }
            public string[] Errors { get; set; } = Array.Empty<string>();
            public DateTime ExecutionTime { get; set; }
            public double Duration { get; set; }
        }

        public class Audience
        {
            public string Role { get; set; } = string.Empty;
            public string TechnicalLevel { get; set; } = "intermediate";
            public string[] Interests { get; set; } = Array.Empty<string>();
            public string[] Constraints { get; set; } = Array.Empty<string>();
        }

        public class UpdateStrategy
        {
            public string Trigger { get; set; } = "scenario-change";
            public bool AutoUpdate { get; set; } = true;
            public string[] NotifyRoles { get; set; } = Array.Empty<string>();
            public string Versioning { get; set; } = "semantic";
        }
    
}
