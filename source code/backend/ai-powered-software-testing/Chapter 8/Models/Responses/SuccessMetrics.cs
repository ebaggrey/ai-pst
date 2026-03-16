
namespace Chapter_8.Models.Responses
{
    public class SuccessMetrics
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public MetricDefinition[] Metrics { get; set; }
        public OverallAssessment OverallAssessment { get; set; }
        public string[] Recommendations { get; set; }
    }

    public class MetricDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; } // "technical", "business", "process", "quality"
        public string Description { get; set; }
        public string MeasurementMethod { get; set; }
        public string TargetValue { get; set; }
        public string CurrentValue { get; set; }
        public string Unit { get; set; } // "percentage", "count", "seconds", "dollars"
        public double? BaselineValue { get; set; }
        public double? ThresholdWarning { get; set; }
        public double? ThresholdCritical { get; set; }
        public string Status { get; set; } // "on-track", "at-risk", "critical", "completed"
        public Trend Trend { get; set; }
        public HistoricalDataPoint[] HistoricalData { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    //public class Trend
    //{
    //    public string Direction { get; set; } // "improving", "stable", "degrading"
    //    public double ChangeRate { get; set; }
    //    public string Interpretation { get; set; }
    //    public DateTime PeriodStart { get; set; }
    //    public DateTime PeriodEnd { get; set; }
    //}

    public class HistoricalDataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public string Annotation { get; set; }
    }

    public class OverallAssessment
    {
        public double OverallScore { get; set; } // 0-100
        public string Grade { get; set; } // "A", "B", "C", "D", "F"
        public string Summary { get; set; }
        public string[] KeyAchievements { get; set; }
        public string[] AreasForImprovement { get; set; }
        public RiskAssessment RiskAssessment { get; set; }
        public Dictionary<string, double> CategoryScores { get; set; }
    }

    //public class RiskAssessment
    //{
    //    public string OverallRiskLevel { get; set; } // "low", "medium", "high", "critical"
    //    public RiskFactor[] RiskFactors { get; set; }
    //    public MitigationStatus[] MitigationStatus { get; set; }
    //    public string[] EmergingRisks { get; set; }
    //}

    public class RiskFactor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Probability { get; set; } // 0-1
        public double Impact { get; set; } // 0-1
        public double RiskScore { get; set; } // Probability * Impact
        public string Status { get; set; } // "active", "mitigated", "accepted"
        public string[] MitigationStrategies { get; set; }
    }

    public class MitigationStatus
    {
        public string RiskId { get; set; }
        public string RiskName { get; set; }
        public string Strategy { get; set; }
        public double ProgressPercentage { get; set; }
        public string Status { get; set; } // "planned", "in-progress", "completed", "failed"
        public string Owner { get; set; }
        public DateTime? TargetCompletionDate { get; set; }
        public string[] Actions { get; set; }
    }
}
