
namespace Chapter_10.Models.Requests
{
    public class MetricDesignRequest
    {
        public string[] BusinessObjectives { get; set; }
        public string[] TestingActivities { get; set; }
        public string[] DesignPrinciples { get; set; }
        public MetricConstraints Constraints { get; set; }
    }

    public class MetricConstraints
    {
        public int MaxMetrics { get; set; }
        public int MinDataPoints { get; set; }
        public string[] RequiredDimensions { get; set; }
        public bool AllowCompositeMetrics { get; set; }
    }
}

// Models/Requests/HealthScoreRequest.cs
namespace Chapter_10.Models.Requests
{
    public class HealthScoreRequest
    {
        public MetricValue[] MetricValues { get; set; }
        public HistoricalBaseline[] HistoricalBaselines { get; set; }
        public string NormalizationMethod { get; set; }
        public string WeightingStrategy { get; set; }
        public double ConfidenceThreshold { get; set; }
    }

    public class MetricValue
    {
        public string MetricId { get; set; }
        public string MetricName { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
    }

    public class HistoricalBaseline
    {
        public string MetricId { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public int SampleSize { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}

// Models/Requests/PredictionRequest.cs
namespace Chapter_10.Models.Requests
{
    public class PredictionRequest
    {
        public MetricValue[] CurrentMetrics { get; set; }
        public HistoricalTrend[] HistoricalTrends { get; set; }
        public int PredictionHorizon { get; set; } // Days
        public double[] ConfidenceIntervals { get; set; }
        public bool IncludeInterventions { get; set; }
    }

    public class HistoricalTrend
    {
        public DateTime Date { get; set; }
        public Dictionary<string, double> MetricValues { get; set; }
        public string Seasonality { get; set; }
    }
}

// Models/Requests/InsightRequest.cs
namespace Chapter_10.Models.Requests
{
    public class InsightRequest
    {
        public MetricValue[] Metrics { get; set; }
        public string[] InsightTypes { get; set; }
        public double ActionabilityThreshold { get; set; }
        public InsightContext Context { get; set; }
    }

    public class InsightContext
    {
        public string[] Stakeholders { get; set; }
        public string[] BusinessGoals { get; set; }
        public Dictionary<string, object> Constraints { get; set; }
        public string PriorityLevel { get; set; }
    }
}

// Models/Requests/OptimizationRequest.cs
namespace Chapter_10.Models.Requests
{
    public class OptimizationRequest
    {
        public MetricDefinition[] CurrentMetrics { get; set; }
        public ResourceConstraint[] ResourceConstraints { get; set; }
        public string[] OptimizationGoals { get; set; }
        public PreservationRule[] PreservationRules { get; set; }
    }

    public class MetricDefinition
    {
        public string MetricId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string CollectionMethod { get; set; }
        public double CollectionCost { get; set; }
        public double BusinessValue { get; set; }
        public string[] Dependencies { get; set; }
    }

    public class ResourceConstraint
    {
        public string ResourceType { get; set; }
        public double MaxAllocation { get; set; }
        public string Unit { get; set; }
        public string Period { get; set; }
    }

    public class PreservationRule
    {
        public string MetricId { get; set; }
        public string Rule { get; set; }
        public string Reason { get; set; }
    }
}