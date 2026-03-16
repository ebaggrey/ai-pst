
// Models/Responses/PipelineResponse.cs
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;

namespace Chapter_11.Models.Responses
{
    public class PipelineResponse
    {
        public string PipelineId { get; set; }
        public Pipeline Pipeline { get; set; }
        public FeedbackConfiguration FeedbackConfiguration { get; set; }
        public ImplementationPlan ImplementationPlan { get; set; }
        public SpectrumCoverage SpectrumCoverage { get; set; }
        public PerformanceProjection PerformanceProjections { get; set; }
        public PipelineRiskAssessment RiskAssessment { get; set; }
        public OptimizationRecommendation[] OptimizationRecommendations { get; set; }
    }

    public class Pipeline
    {
        public PipelineStage[] Stages { get; set; }
        public QualityGate[] QualityGates { get; set; }
        public object? PipelineId { get; internal set; }
    }

    public class PipelineStage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Activities { get; set; }
        public string[] Metrics { get; set; }
    }

    public class FeedbackConfiguration
    {
        public FeedbackChannel[] Channels { get; set; }
    }

    public class FeedbackChannel
    {
        public string Type { get; set; }
        public string Configuration { get; set; }
    }

    public class ImplementationPlan
    {
        public ImplementationTask[] Tasks { get; set; }
    }

    public class ImplementationTask
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }

    public class PerformanceProjection
    {
        public double ExpectedThroughput { get; set; }
        public TimeSpan ExpectedDuration { get; set; }
        public double SuccessRate { get; set; }
    }

    public class PipelineRiskAssessment
    {
        public RiskItem[] Risks { get; set; }
    }

    public class OptimizationRecommendation
    {
        public string Area { get; set; }
        public string Recommendation { get; set; }
        public int Impact { get; set; }
    }
}