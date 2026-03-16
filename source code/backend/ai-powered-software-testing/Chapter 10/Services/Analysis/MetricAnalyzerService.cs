
// Services/Analysis/MetricAnalyzerService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;


namespace Chapter_10.Services.Analysis
{
    public class MetricAnalyzerService : IMetricAnalyzer
    {
        private readonly ILogger<MetricAnalyzerService> _logger;

        public MetricAnalyzerService(ILogger<MetricAnalyzerService> logger)
        {
            _logger = logger;
        }

        public async Task<MetricAnalysis> AnalyzeMetricsForInsightsAsync(
            MetricValue[] metrics,
            string[] insightTypes)
        {
            _logger.LogInformation("Analyzing {MetricCount} metrics for insights", metrics?.Length ?? 0);

            if (metrics == null || metrics.Length == 0)
            {
                return new MetricAnalysis
                {
                    Statistics = Array.Empty<MetricStatistic>(),
                    Anomalies = Array.Empty<Anomaly>(),
                    Correlations = Array.Empty<Correlation>()
                };
            }

            var statistics = CalculateStatistics(metrics);
            var anomalies = DetectAnomalies(metrics);
            var correlations = FindCorrelations(metrics);

            return await Task.FromResult(new MetricAnalysis
            {
                Statistics = statistics,
                Anomalies = anomalies,
                Correlations = correlations
            });
        }

        private MetricStatistic[] CalculateStatistics(MetricValue[] metrics)
        {
            return metrics.GroupBy(m => m.MetricId ?? m.MetricName ?? "unknown")
                .Select(g => new MetricStatistic
                {
                    MetricId = g.Key,
                    Mean = g.Average(m => m.Value),
                    Median = CalculateMedian(g.Select(m => m.Value).ToArray()),
                    StdDev = CalculateStdDev(g.Select(m => m.Value).ToArray()),
                    Trend = CalculateTrend(g.OrderBy(m => m.Timestamp).Select(m => m.Value).ToArray())
                }).ToArray();
        }

        private double CalculateMedian(double[] values)
        {
            if (values.Length == 0) return 0;
            var sorted = values.OrderBy(v => v).ToArray();
            int mid = sorted.Length / 2;
            return sorted.Length % 2 == 0 ? (sorted[mid - 1] + sorted[mid]) / 2 : sorted[mid];
        }

        private double CalculateStdDev(double[] values)
        {
            if (values.Length == 0) return 0;
            var avg = values.Average();
            var variance = values.Select(v => Math.Pow(v - avg, 2)).Average();
            return Math.Sqrt(variance);
        }

        private double CalculateTrend(double[] values)
        {
            if (values.Length < 2) return 0;
            return (values.Last() - values.First()) / values.First();
        }

        private Anomaly[] DetectAnomalies(MetricValue[] metrics)
        {
            var anomalies = new List<Anomaly>();

            foreach (var metric in metrics)
            {
                var stats = CalculateStatistics(new[] { metric });
                if (stats.Length == 0) continue;

                var stdDev = stats[0].StdDev;
                var mean = stats[0].Mean;

                if (stdDev > 0 && Math.Abs(metric.Value - mean) > 2 * stdDev)
                {
                    anomalies.Add(new Anomaly
                    {
                        MetricId = metric.MetricId ?? metric.MetricName ?? "unknown",
                        Timestamp = metric.Timestamp,
                        Value = metric.Value,
                        ExpectedValue = mean,
                        Severity = Math.Abs(metric.Value - mean) / stdDev > 3 ? "high" : "medium"
                    });
                }
            }

            return anomalies.ToArray();
        }

        private Correlation[] FindCorrelations(MetricValue[] metrics)
        {
            var correlations = new List<Correlation>();
            var metricGroups = metrics.GroupBy(m => m.MetricId ?? m.MetricName ?? "unknown").ToList();

            for (int i = 0; i < metricGroups.Count; i++)
            {
                for (int j = i + 1; j < metricGroups.Count; j++)
                {
                    var values1 = metricGroups[i].OrderBy(m => m.Timestamp).Select(m => m.Value).ToArray();
                    var values2 = metricGroups[j].OrderBy(m => m.Timestamp).Select(m => m.Value).ToArray();

                    if (values1.Length == values2.Length && values1.Length > 1)
                    {
                        double correlation = CalculateCorrelation(values1, values2);
                        if (Math.Abs(correlation) > 0.5)
                        {
                            correlations.Add(new Correlation
                            {
                                MetricId1 = metricGroups[i].Key,
                                MetricId2 = metricGroups[j].Key,
                                Coefficient = correlation,
                                Direction = correlation > 0 ? "positive" : "negative"
                            });
                        }
                    }
                }
            }

            return correlations.ToArray();
        }

        private double CalculateCorrelation(double[] values1, double[] values2)
        {
            if (values1.Length != values2.Length || values1.Length == 0)
                return 0;

            double avg1 = values1.Average();
            double avg2 = values2.Average();

            double sumProducts = 0;
            double sumSquares1 = 0;
            double sumSquares2 = 0;

            for (int i = 0; i < values1.Length; i++)
            {
                double diff1 = values1[i] - avg1;
                double diff2 = values2[i] - avg2;
                sumProducts += diff1 * diff2;
                sumSquares1 += diff1 * diff1;
                sumSquares2 += diff2 * diff2;
            }

            if (sumSquares1 == 0 || sumSquares2 == 0)
                return 0;

            return sumProducts / Math.Sqrt(sumSquares1 * sumSquares2);
        }
    }
}
