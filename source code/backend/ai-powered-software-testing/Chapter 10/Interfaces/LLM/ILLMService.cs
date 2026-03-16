
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;

namespace Chapter_10.Interfaces.LLM
{
    public interface ILLMService
    {
        Task<string> GenerateInsightAsync(string prompt, InsightContext context);
        Task<string[]> GenerateRecommendationsAsync(string[] dataPoints, string objective);
        Task<DetectedPattern[]> AnalyzePatternsWithLLMAsync(HistoricalTrend[] trends);
        Task<string> InterpretMetricsAsync(MetricValue[] metrics, string[] objectives);
        Task<OptimizationSuggestion[]> SuggestOptimizationsAsync(MetricDefinition[] metrics, ResourceConstraint[] constraints);
        Task<bool> ValidateMetricDesignAsync(DesignedMetric[] metrics, string[] principles);
        Task<string> GenerateNaturalLanguageExplanationAsync(string metricName, double value, string context);
    }

    public class OptimizationSuggestion
    {
        public string MetricId { get; set; } = string.Empty;
        public string Suggestion { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string Rationale { get; set; } = string.Empty;
    }
}
