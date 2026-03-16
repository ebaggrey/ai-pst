
using Chapter_8.Exceptions;
using Chapter_8.Interfaces;
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;


namespace Chapter_8.Services
{
    public class LegacyHealthMonitor : ILegacyHealthMonitor
    {
        private readonly ILogger<LegacyHealthMonitor> _logger;

        public LegacyHealthMonitor(ILogger<LegacyHealthMonitor> logger)
        {
            _logger = logger;
        }

        public async Task<HealthReport> MonitorHealthAsync(HealthRequest request)
        {
            try
            {
                _logger.LogInformation("Monitoring health for {SystemCount} systems",
                    request.MonitoredSystems?.Length ?? 0);

                // Simulate monitoring work
                await System.Threading.Tasks.Task.Delay(500);

                // Check telemetry consistency
                if (request.TelemetryData != null && HasInconsistentData(request.TelemetryData))
                {
                    throw new TelemetryInconsistencyException(
                        "Telemetry data has gaps or inconsistencies",
                        "Multiple timestamp gaps > 5 minutes",
                        new[] { "Incomplete data for System A", "Out of order timestamps" })
                    {
                        InconsistencyDetails = "Gaps detected: 15:00-15:05, 15:10-15:20",
                        DataQualityIssues = new[] { "Missing metrics", "Timestamp skew" }
                    };
                }

                // Generate health scores
                var healthScores = CalculateHealthScores(request);

                // Detect anomalies
                var anomalies = DetectAnomalies(request, healthScores);

                // Make predictions
                var predictions = MakePredictions(request, anomalies);

                var report = new HealthReport
                {
                    ReportId = Guid.NewGuid().ToString(),
                    GeneratedAt = DateTime.UtcNow,
                    CurrentHealthScores = healthScores.ToDictionary(h => h.SystemId, h => h.Score),
                    Anomalies = anomalies.ToArray(),
                    Trends = AnalyzeTrends(request.TelemetryData).ToArray(),
                    Predictions = predictions.ToArray(),
                    Recommendations = GenerateRecommendations(healthScores, anomalies).ToArray()
                };

                _logger.LogInformation("Health monitoring complete. {AnomalyCount} anomalies detected",
                    anomalies.Count);

                return report;
            }
            catch (Exception ex) when (ex is not TelemetryInconsistencyException)
            {
                _logger.LogError(ex, "Error monitoring system health");
                throw;
            }
        }

        private List<HealthScore> CalculateHealthScores(HealthRequest request)
        {
            var scores = new List<HealthScore>();

            if (request.MonitoredSystems == null) return scores;

            foreach (var system in request.MonitoredSystems)
            {
                var systemData = request.TelemetryData?
                    .Where(t => t.SystemId == system.Id)
                    .ToList();

                var score = new HealthScore
                {
                    SystemId = system.Id,
                    SystemName = system.Name,
                    Score = CalculateSystemScore(systemData),
                    Status = DetermineStatus(systemData),
                    ComponentScores = new Dictionary<string, double>()
                };

                scores.Add(score);
            }

            return scores;
        }

        private double CalculateSystemScore(List<TelemetryDataPoint> data)
        {
            if (data == null || !data.Any()) return 50; // Default score

            // Simple average of metrics
            return data.Average(d => d.Value);
        }

        private string DetermineStatus(List<TelemetryDataPoint> data)
        {
            var avgScore = CalculateSystemScore(data);

            if (avgScore >= 80) return "healthy";
            if (avgScore >= 50) return "warning";
            return "critical";
        }

        private List<Anomaly> DetectAnomalies(HealthRequest request, List<HealthScore> scores)
        {
            var anomalies = new List<Anomaly>();

            foreach (var score in scores.Where(s => s.Status == "critical"))
            {
                anomalies.Add(new Anomaly
                {
                    Id = Guid.NewGuid().ToString(),
                    SystemId = score.SystemId,
                    DetectedAt = DateTime.UtcNow,
                    Metric = "OverallHealth",
                    ExpectedValue = 80,
                    ActualValue = score.Score,
                    Severity = "critical",
                    Description = $"System health critically low: {score.Score:F1}"
                });
            }

            return anomalies;
        }

        private List<Prediction> MakePredictions(HealthRequest request, List<Anomaly> anomalies)
        {
            var predictions = new List<Prediction>();

            if (anomalies.Any(a => a.Severity == "critical"))
            {
                predictions.Add(new Prediction
                {
                    Id = Guid.NewGuid().ToString(),
                    SystemId = anomalies.First().SystemId,
                    PredictedIssue = "System failure within 24 hours",
                    PredictedTimeframe = DateTime.UtcNow.AddHours(24),
                    Confidence = 0.75,
                    SuggestedActions = new[] { "Schedule maintenance", "Prepare rollback plan" }
                });
            }

            return predictions;
        }

        private List<Trend> AnalyzeTrends(TelemetryDataPoint[] telemetry)
        {
            var trends = new List<Trend>();

            if (telemetry == null || !telemetry.Any()) return trends;

            var groupedByMetric = telemetry.GroupBy(t => t.MetricName);

            foreach (var metricGroup in groupedByMetric)
            {
                var values = metricGroup.OrderBy(t => t.Timestamp).Select(t => t.Value).ToList();

                if (values.Count >= 2)
                {
                    var firstHalf = values.Take(values.Count / 2).Average();
                    var secondHalf = values.Skip(values.Count / 2).Average();

                    var changeRate = (secondHalf - firstHalf) / firstHalf;

                    trends.Add(new Trend
                    {
                        Metric = metricGroup.Key,
                        SystemId = metricGroup.First().SystemId,
                        ChangeRate = changeRate,
                        Interpretation = changeRate > 0.1 ? "Increasing" :
                                        changeRate < -0.1 ? "Decreasing" : "Stable"
                    });
                }
            }

            return trends;
        }

        private List<string> GenerateRecommendations(List<HealthScore> scores, List<Anomaly> anomalies)
        {
            var recommendations = new List<string>();

            foreach (var critical in scores.Where(s => s.Status == "critical"))
            {
                recommendations.Add($"Immediate attention required for {critical.SystemName}");
            }

            foreach (var anomaly in anomalies)
            {
                recommendations.Add($"Investigate anomaly in {anomaly.SystemId}: {anomaly.Description}");
            }

            if (!recommendations.Any())
            {
                recommendations.Add("All systems operating normally");
            }

            return recommendations;
        }

        private bool HasInconsistentData(TelemetryDataPoint[] telemetry)
        {
            if (telemetry.Length < 2) return true;

            // Check for timestamp gaps > 5 minutes
            var ordered = telemetry.OrderBy(t => t.Timestamp).ToList();
            for (int i = 1; i < ordered.Count; i++)
            {
                var gap = ordered[i].Timestamp - ordered[i - 1].Timestamp;
                if (gap > TimeSpan.FromMinutes(5))
                {
                    return true;
                }
            }

            return false;
        }
    }
}