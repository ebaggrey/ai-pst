using Chapter_7.Models.Requests;

namespace Chapter_7.Models.Responses
{
    public class OptimizationResponse
    {
        public string OptimizationId { get; set; }
        public PipelineMetrics CurrentPerformance { get; set; }
        public OptimizationOpportunity[] OptimizationOpportunities { get; set; }
        public OptimizationStrategy[] ProposedStrategies { get; set; }
        public TradeOffAnalysis TradeOffAnalysis { get; set; }
        public ImplementationPlan ImplementationPlan { get; set; }
        public ExpectedImprovement[] ExpectedImprovements { get; set; }
        public RiskAssessment RiskAssessment { get; set; }
        public ValidationPlan ValidationPlan { get; set; }
    }

    public class OptimizationOpportunity
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class OptimizationStrategy
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class TradeOffAnalysis
    {
        public TradeOff[] TradeOffs { get; set; }
    }

    public class TradeOff
    {
        public string StrategyA { get; set; }
        public string StrategyB { get; set; }
    }

    public class ImplementationPlan
    {
        public ImplementationStep[] Steps { get; set; }
    }

    public class ImplementationStep
    {
        public int Order { get; set; }
        public string Action { get; set; }
    }

    public class ExpectedImprovement
    {
        public string Metric { get; set; }
        public double Improvement { get; set; }
    }

    public class RiskAssessment
    {
        public Risk[] Risks { get; set; }
    }

    public class Risk
    {
        public string Description { get; set; }
        public double Probability { get; set; }
    }

    public class ValidationPlan
    {
        public string[] TestCases { get; set; }
    }
}
