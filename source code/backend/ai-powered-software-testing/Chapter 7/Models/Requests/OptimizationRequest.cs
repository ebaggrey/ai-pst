
namespace Chapter_7.Models.Requests
{
    public class OptimizationRequest
    {
        public PipelineMetrics CurrentMetrics { get; set; }
        public Bottleneck[] IdentifiedBottlenecks { get; set; }
        public OptimizationGoal[] OptimizationGoals { get; set; }
        public OptimizationConstraints Constraints { get; set; }
    }

    public class PipelineMetrics
    {
        public TimeSpan AverageDuration { get; set; }
        public double SuccessRate { get; set; }
        public double ResourceUtilization { get; set; }
    }

    public class Bottleneck
    {
        public string StageId { get; set; }
        public string Description { get; set; }
        public double ImpactScore { get; set; }
    }

    public class OptimizationGoal
    {
        public string Type { get; set; }
        public double TargetValue { get; set; }
        public Priority Priority { get; set; }
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public class OptimizationConstraints
    {
        public double MaxBudget { get; set; }
        public TimeSpan MaxDowntime { get; set; }
    }
}