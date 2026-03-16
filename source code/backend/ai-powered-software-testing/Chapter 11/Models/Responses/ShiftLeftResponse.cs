
using Chapter_11.Models.Requests;

namespace Chapter_11.Models.Responses
{
    public class ShiftLeftResponse
    {
        public string ArtifactsId { get; set; }
        public RequirementCollection Requirements { get; set; }
        public AcceptanceCriteria[] AcceptanceCriteria { get; set; }
        public TestScenario[] TestScenarios { get; set; }
        public TestDataRequirement[] TestDataRequirements { get; set; }
        public RiskAssessment RiskAssessment { get; set; }
        public CollaborationHistory CollaborationHistory { get; set; }
        public CoverageMetrics CoverageMetrics { get; set; }
        public ImplementationGuidance ImplementationGuidance { get; set; }
        public ValidationChecklist ValidationChecklist { get; set; }
    }

    public class AcceptanceCriteria
    {
        public string Id { get; set; }
        public string RequirementId { get; set; }
        public string Criterion { get; set; }
        public bool IsAutomated { get; set; }
    }

    public class TestScenario
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Steps { get; set; }
        public string ExpectedOutcome { get; set; }
        public string[] Tags { get; set; }
    }

    public class TestDataRequirement
    {
        public string Id { get; set; }
        public string TestScenarioId { get; set; }
        public string DataType { get; set; }
        public object SampleData { get; set; }
    }

    public class RiskAssessment
    {
        public string Id { get; set; }
        public RiskItem[] HighRisks { get; set; }
        public RiskItem[] MediumRisks { get; set; }
        public RiskItem[] LowRisks { get; set; }
    }

    public class RiskItem
    {
        public string Description { get; set; }
        public double Probability { get; set; }
        public double Impact { get; set; }
        public string Mitigation { get; set; }
    }

    public class CollaborationHistory
    {
        public string Mode { get; set; }
        public CollaborationEntry[] Entries { get; set; }
    }

    public class CollaborationEntry
    {
        public DateTime Timestamp { get; set; }
        public string Participant { get; set; }
        public string Action { get; set; }
    }

    public class CoverageMetrics
    {
        public double RequirementCoverage { get; set; }
        public double ScenarioCoverage { get; set; }
        public double RiskCoverage { get; set; }
    }

    public class ImplementationGuidance
    {
        public string[] Steps { get; set; }
        public string[] BestPractices { get; set; }
        public string[] Warnings { get; set; }
    }

    public class ValidationChecklist
    {
        public ChecklistItem[] Items { get; set; }
    }

    public class ChecklistItem
    {
        public string Description { get; set; }
        public bool IsRequired { get; set; }
    }
}