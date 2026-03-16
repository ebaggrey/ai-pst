using Chapter_6.Models.Requests;

namespace Chapter_6.Models.Responses
{
    // Models/Responses.cs
    
        public class BDDCoCreationResponse
        {
            public string SessionId { get; set; } = string.Empty;
            public string Requirement { get; set; } = string.Empty;
            public ConversationRound[] ConversationRounds { get; set; } = Array.Empty<ConversationRound>();
            public BDDScenario[] GeneratedScenarios { get; set; } = Array.Empty<BDDScenario>();
            public string[] AssumptionsChallenged { get; set; } = Array.Empty<string>();
            public string[] ConsensusPoints { get; set; } = Array.Empty<string>();
            public string[] UnresolvedQuestions { get; set; } = Array.Empty<string>();
            public string[] NextSteps { get; set; } = Array.Empty<string>();
            public double ConversationQualityScore { get; set; }
        }

        public class ConversationRound
        {
            public int RoundNumber { get; set; }
            public string[] StakeholderInputs { get; set; } = Array.Empty<string>();
            public string[] Clarifications { get; set; } = Array.Empty<string>();
            public string[] Decisions { get; set; } = Array.Empty<string>();
            public double ConsensusScore { get; set; }
            public BDDConversation UpdatedConversation { get; set; } = new();
        }

        public class BDDConversation
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string[] Participants { get; set; } = Array.Empty<string>();
            public string[] Topics { get; set; } = Array.Empty<string>();
            public string[] Decisions { get; set; } = Array.Empty<string>();
            public string[] OpenQuestions { get; set; } = Array.Empty<string>();
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? EndedAt { get; set; }
        }

        public class AutomationTranslationResponse
        {
            public string TranslationId { get; set; } = string.Empty;
            public BDDScenario OriginalScenario { get; set; } = new();
            public AutomationStep[] AutomationSteps { get; set; } = Array.Empty<AutomationStep>();
            public FrameworkIntegration FrameworkIntegration { get; set; } = new();
            public LivingTest LivingTest { get; set; } = new();
            public QualityReport QualityReport { get; set; } = new();
            public MaintenanceGuidance MaintenanceGuidance { get; set; } = new();
            public EvolutionHook[] EvolutionHooks { get; set; } = Array.Empty<EvolutionHook>();
            public BDDScenario[]? Alternatives { get; set; }
            public string? UnsupportedPattern { get; set; }
            public string? SuggestedRefactoring { get; set; }
        }

    public class AutomationStep
    {
        public string OriginalStep { get; set; } = string.Empty;
        public string AutomationCode { get; set; } = string.Empty;
        public ValidationRule[] ValidationRules { get; set; } = Array.Empty<ValidationRule>();
        public string[] Dependencies { get; set; } = Array.Empty<string>();
        public QualityMetrics QualityMetrics { get; set; } = new();
        public string Code { get; set; } = string.Empty;
    }

    public class FrameworkIntegration
        {
            public string FrameworkSetup { get; set; } = string.Empty;
            public string TestStructure { get; set; } = string.Empty;
            public string[] RequiredPackages { get; set; } = Array.Empty<string>();
            public string[] Configuration { get; set; } = Array.Empty<string>();
        }

        public class LivingTest
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string[] Steps { get; set; } = Array.Empty<string>();
            public string[] Adaptations { get; set; } = Array.Empty<string>();
            public string[] MonitoringPoints { get; set; } = Array.Empty<string>();
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public class QualityMetrics
        {
            public double ComplexityScore { get; set; }
            public double MaintainabilityScore { get; set; }
            public double ReadabilityScore { get; set; }
            public int CyclomaticComplexity { get; set; }
            public int LinesOfCode { get; set; }
        }

        public class QualityReport
        {
            public double CoverageScore { get; set; }
            public double MaintainabilityScore { get; set; }
            public double ReadabilityScore { get; set; }
            public string[] Issues { get; set; } = Array.Empty<string>();
            public string[] Recommendations { get; set; } = Array.Empty<string>();
        }

        public class MaintenanceGuidance
        {
            public string[] CommonIssues { get; set; } = Array.Empty<string>();
            public string[] FixPatterns { get; set; } = Array.Empty<string>();
            public string[] UpdateTriggers { get; set; } = Array.Empty<string>();
            public string[] BestPractices { get; set; } = Array.Empty<string>();
        }

        public class EvolutionHook
        {
            public string HookType { get; set; } = string.Empty;
            public string Trigger { get; set; } = string.Empty;
            public string Action { get; set; } = string.Empty;
            public string[] Dependencies { get; set; } = Array.Empty<string>();
        }

        public class EvolutionResponse
        {
            public string EvolutionId { get; set; } = string.Empty;
            public BDDScenario[] OriginalScenarios { get; set; } = Array.Empty<BDDScenario>();
            public BDDScenario[] EvolvedScenarios { get; set; } = Array.Empty<BDDScenario>();
            public EvolutionRecord[] EvolutionRecords { get; set; } = Array.Empty<EvolutionRecord>();
            public ImpactAnalysis ImpactAnalysis { get; set; } = new();
            public PreservationMetrics PreservationMetrics { get; set; } = new();
            public BreakingChangeHandling BreakingChangeHandling { get; set; } = new();
            public FutureCompatibility FutureCompatibility { get; set; } = new();
        }

        public class EvolutionRecord
        {
            public string OriginalScenarioId { get; set; } = string.Empty;
            public string EvolvedScenarioId { get; set; } = string.Empty;
            public string[] Changes { get; set; } = Array.Empty<string>();
            public string[] Rationale { get; set; } = Array.Empty<string>();
            public double PreservationScore { get; set; }
            public DateTime EvolvedAt { get; set; } = DateTime.UtcNow;
        }

        public class ImpactAnalysis
        {
            public string[] HighImpactChanges { get; set; } = Array.Empty<string>();
            public string[] MediumImpactChanges { get; set; } = Array.Empty<string>();
            public string[] LowImpactChanges { get; set; } = Array.Empty<string>();
            public string[] AffectedAreas { get; set; } = Array.Empty<string>();
            public string[] Risks { get; set; } = Array.Empty<string>();
        }

        public class PreservationMetrics
        {
            public double AveragePreservation { get; set; }
            public double MinPreservation { get; set; }
            public double MaxPreservation { get; set; }
            public string[] WellPreservedAreas { get; set; } = Array.Empty<string>();
            public string[] PoorlyPreservedAreas { get; set; } = Array.Empty<string>();
        }

        public class BreakingChangeHandling
        {
            public string[] SuccessfullyHandled { get; set; } = Array.Empty<string>();
            public string[] PartiallyHandled { get; set; } = Array.Empty<string>();
            public string[] NotHandled { get; set; } = Array.Empty<string>();
            public string[] Workarounds { get; set; } = Array.Empty<string>();
        }

        public class FutureCompatibility
        {
            public double CompatibilityScore { get; set; }
            public string[] FutureProofAreas { get; set; } = Array.Empty<string>();
            public string[] VulnerableAreas { get; set; } = Array.Empty<string>();
            public string[] Recommendations { get; set; } = Array.Empty<string>();
        }

        public class DriftDetectionResponse
        {
            public string DetectionId { get; set; } = string.Empty;
            public BDDScenario[] DocumentedScenarios { get; set; } = Array.Empty<BDDScenario>();
            public ImplementedBehavior[] ImplementedBehavior { get; set; } = Array.Empty<ImplementedBehavior>();
            public DriftFinding[] DriftFindings { get; set; } = Array.Empty<DriftFinding>();
            public string DriftSeverity { get; set; } = "low";
            public CoverageGap[] CoverageGaps { get; set; } = Array.Empty<CoverageGap>();
            public DriftFix[] SuggestedFixes { get; set; } = Array.Empty<DriftFix>();
            public PrioritizedAction[] PrioritizedActions { get; set; } = Array.Empty<PrioritizedAction>();
            public MonitoringRecommendation[] MonitoringRecommendations { get; set; } = Array.Empty<MonitoringRecommendation>();
        }

        public class DriftFinding
        {
            public string Type { get; set; } = string.Empty;
            public string ScenarioId { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Severity { get; set; } = "low";
            public string[] Evidence { get; set; } = Array.Empty<string>();
            public string[] Impact { get; set; } = Array.Empty<string>();
        }

        public class CoverageGap
        {
            public string Area { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string RiskLevel { get; set; } = "medium";
            public string[] AffectedScenarios { get; set; } = Array.Empty<string>();
        }

        public class DriftFix
        {
            public string DriftFindingId { get; set; } = string.Empty;
            public string FixType { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string[] Steps { get; set; } = Array.Empty<string>();
            public string[] Verification { get; set; } = Array.Empty<string>();
        }

        public class PrioritizedAction
        {
            public string Action { get; set; } = string.Empty;
            public string Priority { get; set; } = "medium";
            public string[] Dependencies { get; set; } = Array.Empty<string>();
            public string[] Resources { get; set; } = Array.Empty<string>();
        }

        public class MonitoringRecommendation
        {
            public string Area { get; set; } = string.Empty;
            public string Metric { get; set; } = string.Empty;
            public string Threshold { get; set; } = string.Empty;
            public string[] Triggers { get; set; } = Array.Empty<string>();
        }

        public class DocumentationResponse
        {
            public string DocumentationId { get; set; } = string.Empty;
            public GeneratedDocumentation GeneratedDocumentation { get; set; } = new();
            public ScenarioAnalysis ScenarioAnalysis { get; set; } = new();
            public double AudienceAppropriatenessScore { get; set; }
            public InteractiveFeature[] InteractiveFeatures { get; set; } = Array.Empty<InteractiveFeature>();
            public UpdateMechanism UpdateMechanism { get; set; } = new();
            public AccessPattern[] AccessPatterns { get; set; } = Array.Empty<AccessPattern>();
            public DocumentationQualityMetrics QualityMetrics { get; set; } = new();
        }

        public class GeneratedDocumentation
        {
            public string Format { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public string[] Sections { get; set; } = Array.Empty<string>();
            public string[] Navigation { get; set; } = Array.Empty<string>();
            public string[] InteractiveElements { get; set; } = Array.Empty<string>();
        }

        public class ScenarioAnalysis
        {
            public double HealthScore { get; set; }
            public double CoverageScore { get; set; }
            public string[] HealthyScenarios { get; set; } = Array.Empty<string>();
            public string[] ProblematicScenarios { get; set; } = Array.Empty<string>();
            public string[] Recommendations { get; set; } = Array.Empty<string>();
        }

        public class InteractiveFeature
        {
            public string FeatureType { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string[] Capabilities { get; set; } = Array.Empty<string>();
            public string[] Requirements { get; set; } = Array.Empty<string>();
        }

        public class UpdateMechanism
        {
            public string Type { get; set; } = string.Empty;
            public string Trigger { get; set; } = string.Empty;
            public string[] Processes { get; set; } = Array.Empty<string>();
            public string[] Notifications { get; set; } = Array.Empty<string>();
        }

        public class AccessPattern
        {
            public string Pattern { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string[] UseCases { get; set; } = Array.Empty<string>();
            public string[] BestPractices { get; set; } = Array.Empty<string>();
        }

        public class DocumentationQualityMetrics
        {
            public double ClarityScore { get; set; }
            public double CompletenessScore { get; set; }
            public double AccuracyScore { get; set; }
            public double MaintainabilityScore { get; set; }
            public string[] Issues { get; set; } = Array.Empty<string>();
        }
    
}
