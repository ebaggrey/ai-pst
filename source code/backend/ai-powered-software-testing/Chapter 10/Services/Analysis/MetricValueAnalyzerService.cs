

// Services/Analysis/MetricValueAnalyzerService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;


namespace Chapter_10.Services.Analysis
{
    public class MetricValueAnalyzerService : IMetricValueAnalyzer
    {
        private readonly ILogger<MetricValueAnalyzerService> _logger;

        public MetricValueAnalyzerService(ILogger<MetricValueAnalyzerService> logger)
        {
            _logger = logger;
        }

        public async Task<MetricValueAnalysis> AnalyzeMetricValueAsync(
            MetricDefinition[] metrics,
            PreservationRule[] rules)
        {
            _logger.LogInformation("Analyzing value of {MetricCount} metrics", metrics?.Length ?? 0);

            if (metrics == null || metrics.Length == 0)
            {
                return new MetricValueAnalysis
                {
                    MetricValues = Array.Empty<MetricValueInfo>()
                };
            }

            var metricValues = new List<MetricValueInfo>();

            foreach (var metric in metrics)
            {
                double valueScore = CalculateValueScore(metric);

                metricValues.Add(new MetricValueInfo
                {
                    MetricId = metric.MetricId,
                    Value = valueScore,
                    Category = metric.Category ?? "Uncategorized"
                });
            }

            return await Task.FromResult(new MetricValueAnalysis
            {
                MetricValues = metricValues.ToArray()
            });
        }

     

        private double CalculateValueScore(MetricDefinition metric)
        {
            // Simplified value calculation based on business value and cost
            double businessValueFactor = metric.BusinessValue / 100.0;
            double costFactor = 1.0 - (metric.CollectionCost / 100.0);

            return (businessValueFactor * 0.7 + costFactor * 0.3) * 100;
        }
    }
}