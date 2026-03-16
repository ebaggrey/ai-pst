
using Chapter_11.Models.Requests;

namespace Chapter_11.Models.Responses
{
    public class OrchestrationResponse
    {
        public string OrchestrationId { get; set; }
        public TestSuite TestSuite { get; set; }
        public Orchestration Orchestration { get; set; }
        public ExecutionResult[] ExecutionResults { get; set; }
        public ProcessedResult[] ProcessedResults { get; set; }
        public OrchestrationFeedback Feedback { get; set; }
        public PerformanceMetrics PerformanceMetrics { get; set; }
        public ImprovementRecommendation[] ImprovementRecommendations { get; set; }
        public DocumentationUpdate[] DocumentationUpdates { get; set; }
    }

    public class Orchestration
    {
        public string Id { get; set; }
        public OrchestrationStrategy Strategy { get; set; }
        public TestExecutionPlan[] ExecutionPlans { get; set; }
    }

    public class TestExecutionPlan
    {
        public string TestId { get; set; }
        public int Order { get; set; }
        public string[] Dependencies { get; set; }
    }

    public class ExecutionResult
    {
        public string TestId { get; set; }
        public string Status { get; set; }
        public TimeSpan Duration { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ProcessedResult
    {
        public string TestId { get; set; }
        public object Output { get; set; }
        public string[] Issues { get; set; }
    }

    public class OrchestrationFeedback
    {
        public string Summary { get; set; }
        public FeedbackItem[] Items { get; set; }
    }

    public class FeedbackItem
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class PerformanceMetrics
    {
        public TimeSpan TotalDuration { get; set; }
        public int TotalTests { get; set; }
        public int PassedTests { get; set; }
        public int FailedTests { get; set; }
    }

    public class ImprovementRecommendation
    {
        public string Category { get; set; }
        public string Recommendation { get; set; }
        public int Impact { get; set; }
    }

    public class DocumentationUpdate
    {
        public string TestId { get; set; }
        public string Field { get; set; }
        public string Update { get; set; }
    }
}
