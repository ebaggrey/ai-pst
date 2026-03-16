
using Chapter_8.Interfaces;
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;
using Chapter_8.Orchestrators;
using Chapter_8.Services.LLM;

namespace LegacyConquest.Orchestrators
{
    public class HealthMonitoringOrchestrator : IHealthMonitoringOrchestrator
    {
        private readonly ILegacyHealthMonitor _healthMonitor;
        private readonly ILLMService _llmService;
        private readonly ILogger<HealthMonitoringOrchestrator> _logger;

        public HealthMonitoringOrchestrator(
            ILegacyHealthMonitor healthMonitor,
            ILLMService llmService,
            ILogger<HealthMonitoringOrchestrator> logger)
        {
            _healthMonitor = healthMonitor;
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<HealthResponse> MonitorHealthAsync(HealthRequest request)
        {
            _logger.LogInformation("Starting orchestrated health monitoring for {SystemCount} systems",
                request.MonitoredSystems?.Length ?? 0);

            // Get base health report
            var healthReport = await _healthMonitor.MonitorHealthAsync(request);

            // Analyze telemetry data
            var telemetryAnalysis = await AnalyzeTelemetryWithLLMAsync(
                request.TelemetryData ?? Array.Empty<TelemetryDataPoint>(),
                request.HealthIndicators ?? Array.Empty<HealthIndicator>());

            // Detect anomalies with LLM
            var anomalies = await DetectAnomaliesWithLLMAsync(
                telemetryAnalysis, request.AlertThresholds ?? Array.Empty<AlertThreshold>());

            // Generate health scores
            var healthScores = await CalculateHealthScoresWithLLMAsync(
                request.MonitoredSystems ?? Array.Empty<MonitoredSystem>(),
                telemetryAnalysis);

            // Predict future issues
            var predictions = await PredictFutureIssuesWithLLMAsync(
                telemetryAnalysis, request.MonitoredSystems ?? Array.Empty<MonitoredSystem>());

            // Generate recommendations
            var recommendations = await GenerateHealthRecommendationsWithLLMAsync(
                healthScores, anomalies, predictions, request.MonitoredSystems ?? Array.Empty<MonitoredSystem>());

            // Determine alert status
            var alertStatus = await DetermineAlertStatusWithLLMAsync(anomalies, request.AlertThresholds);

            // Analyze trends
            var trendAnalysis = await AnalyzeTrendsWithLLMAsync(
                telemetryAnalysis, request.MonitoredSystems ?? Array.Empty<MonitoredSystem>());

            // Generate actionable insights
            var actionableInsights = await GenerateActionableInsightsWithLLMAsync(
                healthScores, anomalies, recommendations);

            var response = new HealthResponse
            {
                HealthReportId = healthReport.ReportId,
                MonitoredSystems = request.MonitoredSystems,
                HealthScores = healthScores,
                DetectedAnomalies = anomalies,
                Predictions = predictions,
                Recommendations = recommendations,
                AlertStatus = alertStatus,
                TrendAnalysis = trendAnalysis,
                ActionableInsights = actionableInsights
            };

            return response;
        }

        private async Task<object> AnalyzeTelemetryWithLLMAsync(TelemetryDataPoint[] telemetry, HealthIndicator[] indicators)
        {
            var prompt = $@"
            Analyze this telemetry data:
            Telemetry: {System.Text.Json.JsonSerializer.Serialize(telemetry)}
            Health Indicators: {System.Text.Json.JsonSerializer.Serialize(indicators)}
            
            Return a JSON object with statistical analysis and patterns.
            ";

            return await _llmService.GenerateStructuredContentAsync<object>(prompt) ?? new object();
        }

        private async Task<Anomaly[]> DetectAnomaliesWithLLMAsync(object telemetryAnalysis, AlertThreshold[] thresholds)
        {
            var prompt = $@"
            Detect anomalies in this telemetry analysis:
            Analysis: {System.Text.Json.JsonSerializer.Serialize(telemetryAnalysis)}
            Alert Thresholds: {System.Text.Json.JsonSerializer.Serialize(thresholds)}
            
            Return as JSON array with SystemId, Metric, ExpectedValue, ActualValue, Severity, Description.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<Anomaly[]>(prompt);
            return llmResponse ?? Array.Empty<Anomaly>();
        }

        private async Task<HealthScore[]> CalculateHealthScoresWithLLMAsync(MonitoredSystem[] systems, object telemetryAnalysis)
        {
            var prompt = $@"
            Calculate health scores for these systems:
            Systems: {System.Text.Json.JsonSerializer.Serialize(systems)}
            Telemetry Analysis: {System.Text.Json.JsonSerializer.Serialize(telemetryAnalysis)}
            
            Return as JSON array with SystemId, SystemName, Score (0-100), Status (healthy/warning/critical), ComponentScores.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<HealthScore[]>(prompt);
            return llmResponse ?? systems.Select(s => new HealthScore
            {
                SystemId = s.Id,
                SystemName = s.Name,
                Score = 85,
                Status = "healthy",
                ComponentScores = new Dictionary<string, double>()
            }).ToArray();
        }

        private async Task<Prediction[]> PredictFutureIssuesWithLLMAsync(object telemetryAnalysis, MonitoredSystem[] systems)
        {
            var prompt = $@"
            Predict future issues based on telemetry:
            Analysis: {System.Text.Json.JsonSerializer.Serialize(telemetryAnalysis)}
            Systems: {System.Text.Json.JsonSerializer.Serialize(systems)}
            
            Return as JSON array with SystemId, PredictedIssue, PredictedTimeframe, Confidence (0-1), SuggestedActions.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<Prediction[]>(prompt);
            return llmResponse ?? Array.Empty<Prediction>();
        }

        private async Task<Recommendation[]> GenerateHealthRecommendationsWithLLMAsync(
            HealthScore[] scores,
            Anomaly[] anomalies,
            Prediction[] predictions,
            MonitoredSystem[] systems)
        {
            var prompt = $@"
            Generate health recommendations based on:
            Health Scores: {System.Text.Json.JsonSerializer.Serialize(scores)}
            Anomalies: {System.Text.Json.JsonSerializer.Serialize(anomalies)}
            Predictions: {System.Text.Json.JsonSerializer.Serialize(predictions)}
            Systems: {System.Text.Json.JsonSerializer.Serialize(systems)}
            
            Return as JSON array with Title, Description, Impact, Effort.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<Recommendation[]>(prompt);
            return llmResponse ?? new[]
            {
                new Recommendation
                {
                    Title = "Investigate critical anomalies",
                    Description = "Immediate attention required",
                    Impact = "High",
                    Effort = "Medium"
                }
            };
        }

        private async Task<AlertStatus> DetermineAlertStatusWithLLMAsync(Anomaly[] anomalies, AlertThreshold[] thresholds)
        {
            var prompt = $@"
            Determine alert status based on:
            Anomalies: {System.Text.Json.JsonSerializer.Serialize(anomalies)}
            Alert Thresholds: {System.Text.Json.JsonSerializer.Serialize(thresholds)}
            
            Return as JSON with Level (none/warning/critical), ActiveAlerts (array with Id, SystemId, Type, Message, TriggeredAt), RecommendedResponses.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<AlertStatus>(prompt);

            var criticalCount = anomalies?.Count(a => a.Severity == "critical") ?? 0;
            var warningCount = anomalies?.Count(a => a.Severity == "warning") ?? 0;

            return llmResponse ?? new AlertStatus
            {
                Level = criticalCount > 0 ? "critical" : warningCount > 0 ? "warning" : "none",
                ActiveAlerts = anomalies?.Take(5).Select(a => new Alert
                {
                    Id = a.Id,
                    SystemId = a.SystemId,
                    Type = a.Metric,
                    Message = a.Description,
                    TriggeredAt = a.DetectedAt
                }).ToArray() ?? Array.Empty<Alert>(),
                RecommendedResponses = criticalCount > 0
                    ? new[] { "Immediate investigation required", "Alert on-call team" }
                    : warningCount > 0
                        ? new[] { "Review during business hours", "Monitor closely" }
                        : new[] { "No action required" }
            };
        }

        private async Task<TrendAnalysis> AnalyzeTrendsWithLLMAsync(object telemetryAnalysis, MonitoredSystem[] systems)
        {
            var prompt = $@"
            Analyze trends in telemetry data:
            Analysis: {System.Text.Json.JsonSerializer.Serialize(telemetryAnalysis)}
            Systems: {System.Text.Json.JsonSerializer.Serialize(systems)}
            
            Return as JSON with IncreasingTrends, DecreasingTrends, StableTrends (each array with Metric, SystemId, ChangeRate, Interpretation).
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<TrendAnalysis>(prompt);
            return llmResponse ?? new TrendAnalysis
            {
                IncreasingTrends = Array.Empty<Trend>(),
                DecreasingTrends = Array.Empty<Trend>(),
                StableTrends = Array.Empty<Trend>()
            };
        }

        private async Task<ActionableInsight[]> GenerateActionableInsightsWithLLMAsync(
            HealthScore[] scores,
            Anomaly[] anomalies,
            Recommendation[] recommendations)
        {
            var prompt = $@"
            Generate actionable insights from:
            Health Scores: {System.Text.Json.JsonSerializer.Serialize(scores)}
            Anomalies: {System.Text.Json.JsonSerializer.Serialize(anomalies)}
            Recommendations: {System.Text.Json.JsonSerializer.Serialize(recommendations)}
            
            Return as JSON array with Category, Insight, RecommendedAction, Priority.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<ActionableInsight[]>(prompt);
            return llmResponse ?? new[]
            {
                new ActionableInsight
                {
                    Category = "Performance",
                    Insight = "Response times increasing",
                    RecommendedAction = "Scale resources",
                    Priority = "High"
                }
            };
        }
    }
}
