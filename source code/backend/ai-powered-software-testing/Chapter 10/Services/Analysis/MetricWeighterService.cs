//namespace Chapter_10.Services.Analysis
//{
//    public class MetricWeighterService
//    {
//    }
//}

// Services/Analysis/MetricWeighterService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;


namespace Chapter_10.Services.Analysis
{
    public class MetricWeighterService : IMetricWeighter
    {
        private readonly ILogger<MetricWeighterService> _logger;
        private readonly IConfiguration _configuration;

        public MetricWeighterService(
            ILogger<MetricWeighterService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<WeightedMetric[]> ApplyWeightingAsync(
            NormalizedMetric[] metrics,
            string strategy,
            HistoricalBaseline[] baselines)
        {
            _logger.LogInformation("Applying {Strategy} weighting strategy to {Count} metrics",
                strategy ?? "default", metrics?.Length ?? 0);

            if (metrics == null || metrics.Length == 0)
            {
                _logger.LogWarning("No metrics provided for weighting");
                return Array.Empty<WeightedMetric>();
            }

            var weightingStrategy = DetermineWeightingStrategy(strategy);
            var weightedMetrics = new List<WeightedMetric>();
            var weights = new double[metrics.Length];

            try
            {
                // Calculate weights based on strategy
                weights = weightingStrategy switch
                {
                    WeightingStrategy.Equal => CalculateEqualWeights(metrics.Length),
                    WeightingStrategy.Statistical => await CalculateStatisticalWeights(metrics, baselines),
                    WeightingStrategy.BusinessValue => await CalculateBusinessValueWeights(metrics, baselines),
                    WeightingStrategy.InverseVariance => await CalculateInverseVarianceWeights(metrics, baselines),
                    WeightingStrategy.ExpertJudgment => await CalculateExpertJudgmentWeights(metrics, baselines),
                    WeightingStrategy.Dynamic => await CalculateDynamicWeights(metrics, baselines),
                    _ => CalculateEqualWeights(metrics.Length)
                };

                // Normalize weights to sum to 1
                var totalWeight = weights.Sum();
                if (Math.Abs(totalWeight) > 0.0001)
                {
                    for (int i = 0; i < weights.Length; i++)
                    {
                        weights[i] /= totalWeight;
                    }
                }

                // Create weighted metrics
                for (int i = 0; i < metrics.Length; i++)
                {
                    var metric = metrics[i];
                    var weightedMetric = new WeightedMetric
                    {
                        MetricId = metric.MetricId,
                        MetricName = metric.MetricName,
                        Value = metric.NormalizedValue,
                        Weight = weights[i],
                        WeightingStrategy = weightingStrategy.ToString(),
                        Confidence = CalculateWeightConfidence(metric, weights[i], baselines)
                    };

                    weightedMetrics.Add(weightedMetric);

                    _logger.LogDebug("Weighted metric {MetricName}: weight = {Weight:F4}, confidence = {Confidence:F2}",
                        metric.MetricName, weights[i], weightedMetric.Confidence);
                }

                // Validate weight distribution
                ValidateWeightDistribution(weightedMetrics.ToArray());

                _logger.LogInformation("Successfully applied {Strategy} weighting to {Count} metrics",
                    weightingStrategy, weightedMetrics.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying weighting strategy, falling back to equal weights");

                // Fallback to equal weights
                weights = CalculateEqualWeights(metrics.Length);
                for (int i = 0; i < metrics.Length; i++)
                {
                    weightedMetrics.Add(new WeightedMetric
                    {
                        MetricId = metrics[i].MetricId,
                        MetricName = metrics[i].MetricName,
                        Value = metrics[i].NormalizedValue,
                        Weight = weights[i],
                        WeightingStrategy = "equal-fallback",
                        Confidence = 0.5
                    });
                }
            }

            return await Task.FromResult(weightedMetrics.ToArray());
        }

        private WeightingStrategy DetermineWeightingStrategy(string? strategy)
        {
            if (string.IsNullOrEmpty(strategy))
            {
                var defaultStrategy = _configuration["Analysis:WeightingStrategies:Default"];
                return Enum.TryParse<WeightingStrategy>(defaultStrategy, true, out var parsed)
                    ? parsed
                    : WeightingStrategy.Equal;
            }

            return strategy.ToLower() switch
            {
                "equal" => WeightingStrategy.Equal,
                "statistical" or "stat" => WeightingStrategy.Statistical,
                "business-value" or "businessvalue" or "value" => WeightingStrategy.BusinessValue,
                "inverse-variance" or "inversevariance" => WeightingStrategy.InverseVariance,
                "expert" or "expert-judgment" => WeightingStrategy.ExpertJudgment,
                "dynamic" => WeightingStrategy.Dynamic,
                _ => WeightingStrategy.Equal
            };
        }

        private double[] CalculateEqualWeights(int count)
        {
            if (count == 0) return Array.Empty<double>();
            var weight = 1.0 / count;
            return Enumerable.Repeat(weight, count).ToArray();
        }

        private async Task<double[]> CalculateStatisticalWeights(
            NormalizedMetric[] metrics,
            HistoricalBaseline[] baselines)
        {
            _logger.LogDebug("Calculating statistical weights");

            var weights = new double[metrics.Length];

            for (int i = 0; i < metrics.Length; i++)
            {
                var baseline = baselines?.FirstOrDefault(b =>
                    b.MetricId?.Equals(metrics[i].MetricId, StringComparison.OrdinalIgnoreCase) == true);

                if (baseline != null && baseline.StandardDeviation > 0)
                {
                    // Lower variance = higher weight (more reliable)
                    weights[i] = 1.0 / (1.0 + baseline.StandardDeviation);
                }
                else
                {
                    weights[i] = 1.0; // Default weight
                }
            }

            return await Task.FromResult(weights);
        }

        private async Task<double[]> CalculateBusinessValueWeights(
            NormalizedMetric[] metrics,
            HistoricalBaseline[] baselines)
        {
            _logger.LogDebug("Calculating business value weights");

            var weights = new double[metrics.Length];

            // Simulate business value calculation
            // In real implementation, this would come from business rules or external system
            for (int i = 0; i < metrics.Length; i++)
            {
                var metric = metrics[i];

                // Higher weight for metrics related to key business areas
                weights[i] = metric.MetricName?.ToLower() switch
                {
                    string name when name.Contains("revenue") => 1.5,
                    string name when name.Contains("customer") => 1.3,
                    string name when name.Contains("quality") => 1.2,
                    string name when name.Contains("performance") => 1.1,
                    _ => 1.0
                };
            }

            return await Task.FromResult(weights);
        }

        private async Task<double[]> CalculateInverseVarianceWeights(
            NormalizedMetric[] metrics,
            HistoricalBaseline[] baselines)
        {
            _logger.LogDebug("Calculating inverse variance weights");

            var weights = new double[metrics.Length];

            for (int i = 0; i < metrics.Length; i++)
            {
                var baseline = baselines?.FirstOrDefault(b =>
                    b.MetricId?.Equals(metrics[i].MetricId, StringComparison.OrdinalIgnoreCase) == true);

                if (baseline != null && baseline.StandardDeviation > 0)
                {
                    // Weight inversely proportional to variance
                    var variance = baseline.StandardDeviation * baseline.StandardDeviation;
                    weights[i] = 1.0 / variance;
                }
                else
                {
                    weights[i] = 1.0;
                }
            }

            return await Task.FromResult(weights);
        }

        private async Task<double[]> CalculateExpertJudgmentWeights(
            NormalizedMetric[] metrics,
            HistoricalBaseline[] baselines)
        {
            _logger.LogDebug("Calculating expert judgment weights");

            var weights = new double[metrics.Length];

            // Simulate expert judgment input
            // In real implementation, this would come from a configuration or external source
            for (int i = 0; i < metrics.Length; i++)
            {
                // Default weights based on metric importance tiers
                weights[i] = (i % 3) switch
                {
                    0 => 1.5, // Tier 1 - Most important
                    1 => 1.0, // Tier 2 - Medium importance
                    2 => 0.5, // Tier 3 - Least important
                    _ => 1.0
                };
            }

            return await Task.FromResult(weights);
        }

        private async Task<double[]> CalculateDynamicWeights(
            NormalizedMetric[] metrics,
            HistoricalBaseline[] baselines)
        {
            _logger.LogDebug("Calculating dynamic weights");

            var weights = new double[metrics.Length];

            // Dynamic weighting based on multiple factors
            for (int i = 0; i < metrics.Length; i++)
            {
                var metric = metrics[i];
                var baseline = baselines?.FirstOrDefault(b =>
                    b.MetricId?.Equals(metric.MetricId, StringComparison.OrdinalIgnoreCase) == true);

                double weight = 1.0;

                // Factor 1: Statistical reliability
                if (baseline != null)
                {
                    weight *= 1.0 / (1.0 + baseline.StandardDeviation);
                    weight *= Math.Min(1.0, baseline.SampleSize / 100.0);
                }

                // Factor 2: Current performance deviation
                if (baseline != null && baseline.Mean > 0)
                {
                    var deviation = Math.Abs(metric.NormalizedValue - baseline.Mean) / baseline.Mean;
                    weight *= 1.0 + deviation; // Higher weight for metrics deviating from baseline
                }

                // Factor 3: Metric name based (simulated business relevance)
                if (metric.MetricName?.ToLower().Contains("critical") == true)
                {
                    weight *= 1.2;
                }

                weights[i] = Math.Max(0.1, Math.Min(5.0, weight)); // Clamp between 0.1 and 5.0
            }

            return await Task.FromResult(weights);
        }

        private double CalculateWeightConfidence(
            NormalizedMetric metric,
            double weight,
            HistoricalBaseline[] baselines)
        {
            var baseline = baselines?.FirstOrDefault(b =>
                b.MetricId?.Equals(metric.MetricId, StringComparison.OrdinalIgnoreCase) == true);

            if (baseline == null) return 0.5;

            // Confidence based on sample size and stability
            var sampleSizeConfidence = Math.Min(1.0, baseline.SampleSize / 100.0);
            var stabilityConfidence = 1.0 / (1.0 + baseline.StandardDeviation);

            return (sampleSizeConfidence * 0.6 + stabilityConfidence * 0.4);
        }

        private void ValidateWeightDistribution(WeightedMetric[] weightedMetrics)
        {
            var totalWeight = weightedMetrics.Sum(w => w.Weight);

            if (Math.Abs(totalWeight - 1.0) > 0.01)
            {
                _logger.LogWarning("Weight distribution sum is {TotalWeight:F4}, expected 1.0", totalWeight);
            }

            var maxWeight = weightedMetrics.Max(w => w.Weight);
            var minWeight = weightedMetrics.Min(w => w.Weight);

            if (maxWeight / minWeight > 10)
            {
                _logger.LogWarning("Large weight disparity detected: max/min ratio = {Ratio:F2}", maxWeight / minWeight);
            }
        }
    }

    public enum WeightingStrategy
    {
        Equal,
        Statistical,
        BusinessValue,
        InverseVariance,
        ExpertJudgment,
        Dynamic
    }
}