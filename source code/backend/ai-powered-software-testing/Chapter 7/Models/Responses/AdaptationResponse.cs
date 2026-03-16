namespace Chapter_7.Models.Responses
{
    public class AdaptationResponse
    {
        public string AdaptationId { get; set; }
        public string ChangeType { get; set; }
        public AdaptationPlan AdaptationPlan { get; set; }
        public ValidationResult[] ValidationResults { get; set; }
        public RollbackPlan RollbackPlan { get; set; }
        public EffortEstimate EffortEstimate { get; set; }
        public ImplementationStep[] ImplementationSteps { get; set; }
        public TestingStrategy TestingStrategy { get; set; }
        public CommunicationPlan CommunicationPlan { get; set; }
    }

    public class AdaptationPlan
    {
        public AdaptationStep[] Steps { get; set; }
    }

    public class AdaptationStep
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class ValidationResult
    {
        public string RuleId { get; set; }
        public bool Passed { get; set; }
    }

    public class RollbackPlan
    {
        public RollbackStep[] Steps { get; set; }
    }

    public class RollbackStep
    {
        public int Order { get; set; }
        public string Action { get; set; }
    }

    public class EffortEstimate
    {
        public TimeSpan Duration { get; set; }
        public int Complexity { get; set; }
    }

    public class TestingStrategy
    {
        public string[] TestTypes { get; set; }
    }

    public class CommunicationPlan
    {
        public string[] Stakeholders { get; set; }
    }
}
