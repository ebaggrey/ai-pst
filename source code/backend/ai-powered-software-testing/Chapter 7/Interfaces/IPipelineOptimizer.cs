
using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;
namespace Chapter_7.Interfaces
{
    
    public interface IPipelineOptimizer
    {
        Task<OptimizationOpportunity[]> IdentifyOptimizationOpportunitiesAsync(
            PerformanceAnalysis performanceAnalysis,
            Bottleneck[] identifiedBottlenecks);

        Task<OptimizationStrategy> GenerateOptimizationStrategyAsync(
            OptimizationOpportunity[] opportunities,
            OptimizationGoal goal,
            OptimizationConstraints constraints);
    }

    public class PerformanceAnalysis
    {
        public Bottleneck[] Bottlenecks { get; set; }
        public double EfficiencyScore { get; set; }
    }
}
