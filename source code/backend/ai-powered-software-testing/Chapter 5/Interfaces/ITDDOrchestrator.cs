using Chapter_5.Models.Domain;
using Chapter_5.Models.Requests;

namespace Chapter_5.Interfaces
{
    // Interfaces/ITDDOrchestrator.cs
    public interface ITDDOrchestrator
    {
        Task<TestResult[]> RunVirtualTestsAsync(CodeSnippet implementation, GeneratedTest test);
        Task<TDDCycle> CreateNextCycleAsync(TestedImplementation implementation, ImplementationRequest request);
        Task<TestValidation> ValidateTestIsActuallyFailingAsync(GeneratedTest test, TDDRequest request);
    }

    // Interfaces/ITestFirstGenerator.cs
    public interface ITestFirstGenerator
    {
        Task<GeneratedTest> GenerateFailingTestAsync(TDDRequest request);
        Task<GeneratedTest> AdjustTestToFailAsync(GeneratedTest test, TestValidation validation);
        Task<TestValidation> ValidateTestAsync(GeneratedTest test);
    }

    // Interfaces/IImplementationGenerator.cs
    public interface IImplementationGenerator
    {
        Task<ImplementationSuggestion[]> GenerateImplementationSuggestionsAsync(GeneratedTest failingTest, TDDRequest request);
        Task<CodeSnippet[]> GenerateImplementationsAsync(ImplementationRequest request);
        Task<ImplementationAnalysis> AnalyzeImplementationQualityAsync(CodeSnippet implementation, TestResult[] testResults);
    }

    // Interfaces/IRefactoringAdvisor.cs
    public interface IRefactoringAdvisor
    {
        Task<RefactoringHint[]> PredictRefactoringNeedsAsync(ImplementationSuggestion[] implementations, TDDRequest request);
        Task<RefactoringPlan> CreateRefactoringPlanAsync(CodeSnippet workingCode, RefactoringGoal[] goals, Constraint[] constraints);
        Task<RefactoringOpportunity[]> IdentifyRefactoringOpportunitiesAsync(TestedImplementation[] implementations);
    }

    // Interfaces/IFuturePredictor.cs
    public interface IFuturePredictor
    {
        Task<ChangePrediction[]> PredictChangesAsync(CodeAnalysis analysis, RoadmapAnalysis roadmapAnalysis, TimeHorizon horizon);
        Task<FutureTestRecommendation[]> GenerateTestsForPredictedChangeAsync(ChangePrediction prediction, FuturePredictionRequest request);
        Task<RiskMitigationStrategy[]> GenerateRiskMitigationStrategiesAsync(ChangePrediction[] predictions, List<FutureTestRecommendation> futureTests);
    }
}
