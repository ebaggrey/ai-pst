

using Chapter_8.Models.Requests;

namespace Chapter_8.Models.Responses
{
    // Models/Responses/HealthResponse.cs
    
        public class HealthResponse
        {
            public string HealthReportId { get; set; }
            public MonitoredSystem[] MonitoredSystems { get; set; }
            public HealthScore[] HealthScores { get; set; }
            public Anomaly[] DetectedAnomalies { get; set; }
            public Prediction[] Predictions { get; set; }
            public Recommendation[] Recommendations { get; set; }
            public AlertStatus AlertStatus { get; set; }
            public TrendAnalysis TrendAnalysis { get; set; }
            public ActionableInsight[] ActionableInsights { get; set; }
       
    }

        public class HealthScore
        {
            public string SystemId { get; set; }
            public string SystemName { get; set; }
            public double Score { get; set; } // 0-100
            public string Status { get; set; } // "healthy", "warning", "critical"
            public Dictionary<string, double> ComponentScores { get; set; }
        }

        public class Anomaly
        {
            public string Id { get; set; }
            public string SystemId { get; set; }
            public DateTime DetectedAt { get; set; }
            public string Metric { get; set; }
            public double ExpectedValue { get; set; }
            public double ActualValue { get; set; }
            public string Severity { get; set; }
            public string Description { get; set; }
        }

        public class Prediction
        {
            public string Id { get; set; }
            public string SystemId { get; set; }
            public string PredictedIssue { get; set; }
            public DateTime PredictedTimeframe { get; set; }
            public double Confidence { get; set; } // 0-1
            public string[] SuggestedActions { get; set; }
        }

        public class Recommendation
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Impact { get; set; }
            public string Effort { get; set; }
        }

        public class AlertStatus
        {
            public string Level { get; set; } // "none", "warning", "critical"
            public Alert[] ActiveAlerts { get; set; }
            public string[] RecommendedResponses { get; set; }
        }

        public class Alert
        {
            public string Id { get; set; }
            public string SystemId { get; set; }
            public string Type { get; set; }
            public string Message { get; set; }
            public DateTime TriggeredAt { get; set; }
        }

        public class TrendAnalysis
        {
            public Trend[] IncreasingTrends { get; set; }
            public Trend[] DecreasingTrends { get; set; }
            public Trend[] StableTrends { get; set; }
        }

        public class Trend
        {
            public string Metric { get; set; }
            public string SystemId { get; set; }
            public double ChangeRate { get; set; }
            public string Interpretation { get; set; }
        }

        public class ActionableInsight
        {
            public string Category { get; set; }
            public string Insight { get; set; }
            public string RecommendedAction { get; set; }
            public string Priority { get; set; }
        }
    
}
