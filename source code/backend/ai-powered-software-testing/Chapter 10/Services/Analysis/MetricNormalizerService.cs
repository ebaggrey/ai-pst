
// Services/Analysis/MetricNormalizerService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;


namespace Chapter_10.Services.Analysis
{
    public class MetricNormalizerService : IMetricNormalizer
    {
        private readonly ILogger<MetricNormalizerService> _logger;
        private readonly IConfiguration _configuration;

        public MetricNormalizerService(
            ILogger<MetricNormalizerService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<NormalizedMetric[]> NormalizeMetricsAsync(
            MetricValue[] values,
            HistoricalBaseline[] baselines,
            string method)
        {
            _logger.LogInformation("Normalizing {Count} metrics using {Method} method",
                values?.Length ?? 0, method ?? "default");

            if (values == null || values.Length == 0)
            {
                _logger.LogWarning("No metric values provided for normalization");
                return Array.Empty<NormalizedMetric>();
            }

            var normalizedMetrics = new List<NormalizedMetric>();
            var normalizationMethod = DetermineNormalizationMethod(method);

            foreach (var value in values)
            {
                try
                {
                    var baseline = baselines?.FirstOrDefault(b =>
                        b.MetricId?.Equals(value.MetricId, StringComparison.OrdinalIgnoreCase) == true);

                    double normalizedValue = normalizationMethod switch
                    {
                        NormalizationMethod.MinMax => MinMaxNormalization(value.Value, baseline),
                        NormalizationMethod.ZScore => ZScoreNormalization(value.Value, baseline),
                        NormalizationMethod.DecimalScaling => DecimalScalingNormalization(value.Value, baseline),
                        NormalizationMethod.Robust => RobustNormalization(value.Value, baseline),
                        NormalizationMethod.UnitVector => UnitVectorNormalization(value.Value, baseline),
                        _ => DefaultNormalization(value.Value, baseline)
                    };

                    // Ensure normalized value is within reasonable bounds
                    normalizedValue = SanitizeNormalizedValue(normalizedValue);

                    normalizedMetrics.Add(new NormalizedMetric
                    {
                        MetricId = value.MetricId ?? Guid.NewGuid().ToString(),
                        MetricName = value.MetricName ?? "Unknown Metric",
                        NormalizedValue = normalizedValue,
                        OriginalValue = value.Value,
                        NormalizationMethod = normalizationMethod.ToString()
                    });

                    _logger.LogDebug("Normalized metric {MetricName} from {OriginalValue:F2} to {NormalizedValue:F2}",
                        value.MetricName, value.Value, normalizedValue);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error normalizing metric {MetricId}", value.MetricId);

                    // Add with default value on error
                    normalizedMetrics.Add(new NormalizedMetric
                    {
                        MetricId = value.MetricId ?? Guid.NewGuid().ToString(),
                        MetricName = value.MetricName ?? "Unknown Metric",
                        NormalizedValue = 50.0, // Default middle value
                        OriginalValue = value.Value,
                        NormalizationMethod = "error-fallback"
                    });
                }
            }

            _logger.LogInformation("Successfully normalized {Count} metrics", normalizedMetrics.Count);
            return await Task.FromResult(normalizedMetrics.ToArray());
        }

        private NormalizationMethod DetermineNormalizationMethod(string? method)
        {
            if (string.IsNullOrEmpty(method))
            {
                var defaultMethod = _configuration["Analysis:NormalizationMethods:Default"];
                return Enum.TryParse<NormalizationMethod>(defaultMethod, true, out var parsed)
                    ? parsed
                    : NormalizationMethod.MinMax;
            }

            return method.ToLower() switch
            {
                "min-max" or "minmax" => NormalizationMethod.MinMax,
                "z-score" or "zscore" => NormalizationMethod.ZScore,
                "decimal-scaling" or "decimalscaling" => NormalizationMethod.DecimalScaling,
                "robust" => NormalizationMethod.Robust,
                "unit-vector" or "unitvector" => NormalizationMethod.UnitVector,
                _ => NormalizationMethod.MinMax
            };
        }

        private double MinMaxNormalization(double value, HistoricalBaseline? baseline)
        {
            if (baseline == null) return value;

            var min = baseline.Min;
            var max = baseline.Max;

            if (Math.Abs(max - min) < 0.0001) return 0.5; // Avoid division by zero

            return (value - min) / (max - min) * 100;
        }

        private double ZScoreNormalization(double value, HistoricalBaseline? baseline)
        {
            if (baseline == null || Math.Abs(baseline.StandardDeviation) < 0.0001)
                return value;

            return (value - baseline.Mean) / baseline.StandardDeviation;
        }

        private double DecimalScalingNormalization(double value, HistoricalBaseline? baseline)
        {
            if (baseline == null) return value;

            var maxAbsValue = Math.Max(Math.Abs(baseline.Max), Math.Abs(baseline.Min));
            if (maxAbsValue < 0.0001) return value;

            var j = Math.Ceiling(Math.Log10(maxAbsValue));
            return value / Math.Pow(10, j);
        }

        private double RobustNormalization(double value, HistoricalBaseline? baseline)
        {
            if (baseline == null) return value;

            var median = baseline.Mean; // Using mean as proxy for median
            var iqr = baseline.StandardDeviation * 1.35; // Approximate IQR from std dev

            if (Math.Abs(iqr) < 0.0001) return value;

            return (value - median) / iqr;
        }

        private double UnitVectorNormalization(double value, HistoricalBaseline? baseline)
        {
            if (baseline == null) return value;

            var magnitude = Math.Sqrt(baseline.Mean * baseline.Mean + baseline.StandardDeviation * baseline.StandardDeviation);
            if (Math.Abs(magnitude) < 0.0001) return value;

            return value / magnitude;
        }

        private double DefaultNormalization(double value, HistoricalBaseline? baseline)
        {
            // Simple percentage of target if available
            if (baseline?.Mean > 0)
                return (value / baseline.Mean) * 100;

            return value;
        }

        private double SanitizeNormalizedValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return 50.0;

            // Clip to reasonable range for most use cases
            return Math.Max(0, Math.Min(100, value));
        }
    }

    public enum NormalizationMethod
    {
        MinMax,
        ZScore,
        DecimalScaling,
        Robust,
        UnitVector
    }
}