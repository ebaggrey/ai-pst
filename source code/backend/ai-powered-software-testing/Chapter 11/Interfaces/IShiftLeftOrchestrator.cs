
// Interfaces/IShiftLeftOrchestrator.cs
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;

namespace Chapter_11.Interfaces
{
    public interface IShiftLeftOrchestrator
    {
        Task<AcceptanceCriteria[]> CoCreateAcceptanceCriteriaAsync(
            RequirementCollection requirements,
            CollaborationMode mode);

        Task<TestScenario[]> GenerateTestScenariosAsync(
            RequirementCollection requirements,
            DesignDocument[] designDocuments,
            int shiftDepth);

        Task<TestDataRequirement[]> DefineTestDataRequirementsAsync(
            TestScenario[] testScenarios,
            RequirementCollection requirements);

        Task<RiskAssessment> AssessRisksAsync(
            RequirementCollection requirements,
            TestScenario[] testScenarios);
    }
}