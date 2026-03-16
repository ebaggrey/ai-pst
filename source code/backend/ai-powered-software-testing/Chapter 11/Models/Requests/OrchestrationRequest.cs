
// Models/Requests/OrchestrationRequest.cs
namespace Chapter_11.Models.Requests
{
    public class OrchestrationRequest
    {
        public TestSuite TestSuite { get; set; }
        public ExecutionContext ExecutionContext { get; set; }
        public OrchestrationStrategy OrchestrationStrategy { get; set; }
        public FailureResponseBehavior FailureResponse { get; set; }
    }

    public class TestSuite
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Test[] Tests { get; set; }
    }

    public class Test
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string[] Tags { get; set; }
    }

    public class ExecutionContext
    {
        public string Environment { get; set; }
        public ExecutionCapability Capabilities { get; set; }
        public TimeSpan Timeout { get; set; }
    }

    public class ExecutionCapability
    {
        public int MaxParallelTests { get; set; }
        public string[] SupportedEnvironments { get; set; }
    }

    public enum OrchestrationStrategy
    {
        Sequential,
        Parallel,
        Adaptive,
        PriorityBased
    }

    public enum FailureResponseBehavior
    {
        Stop,
        Continue,
        ReportOnly,
        Retry
    }
}