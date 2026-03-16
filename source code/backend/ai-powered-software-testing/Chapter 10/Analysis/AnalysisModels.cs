
namespace Chapter_10.Analysis
{
    public class ObjectiveAnalysis
    {
        public KeyObjective[] KeyObjectives { get; set; }
        public string[] MeasurableAspects { get; set; }
        public string[] AmbiguousObjectives { get; set; }
    }

    public class KeyObjective
    {
        public string Name { get; set; }
        public double TargetValue { get; set; }
        public string Unit { get; set; }
        public DateTime TargetDate { get; set; }
    }

    public class ActivityValueMapping
    {
        public ActivityValue[] Mappings { get; set; }
    }

    public class ActivityValue
    {
        public string Activity { get; set; }
        public string[] RelatedObjectives { get; set; }
        public double ValueContribution { get; set; }
    }

    public class NormalizedMetric
    {
        public string MetricId { get; set; } = string.Empty;
        public string MetricName { get; set; } = string.Empty;
        public double NormalizedValue { get; set; }
        public double OriginalValue { get; set; }
        public string NormalizationMethod { get; set; } = string.Empty;
    }

    public class WeightedMetric
    {
        public string MetricId { get; set; } = string.Empty;
        public string MetricName { get; set; } = string.Empty;
        public double Value { get; set; }
        public double Weight { get; set; }
        public string WeightingStrategy { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    public class MetricAnalysis
    {
        public MetricStatistic[] Statistics { get; set; }
        public Anomaly[] Anomalies { get; set; }
        public Correlation[] Correlations { get; set; }
    }

    public class MetricStatistic
    {
        public string MetricId { get; set; }
        public double Mean { get; set; }
        public double Median { get; set; }
        public double StdDev { get; set; }
        public double Trend { get; set; }
    }

    public class Anomaly
    {
        public string MetricId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public double ExpectedValue { get; set; }
        public string Severity { get; set; }
    }

    public class Correlation
    {
        public string MetricId1 { get; set; }
        public string MetricId2 { get; set; }
        public double Coefficient { get; set; }
        public string Direction { get; set; }
    }

    public class MetricValueAnalysis
    {
        public MetricValueInfo[] MetricValues { get; set; }
    }

    public class MetricValueInfo
    {
        public string MetricId { get; set; }
        public double Value { get; set; }
        public string Category { get; set; }
    }

    public class CollectionCostAnalysis
    {
        public MetricCost[] MetricCosts { get; set; }
        public double TotalCost { get; set; }
    }

    public class MetricCost
    {
        public string MetricId { get; set; }
        public double Cost { get; set; }
        public string CostDriver { get; set; }
    }
}