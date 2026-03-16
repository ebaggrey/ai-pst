namespace Chapter_8.Models.Requests
{
    // Models/Requests/HealthRequest.cs
   
        public class HealthRequest
        {
            public MonitoredSystem[] MonitoredSystems { get; set; }
            public TelemetryDataPoint[] TelemetryData { get; set; }
            public HealthIndicator[] HealthIndicators { get; set; }
            public AlertThreshold[] AlertThresholds { get; set; }
        }

        public class MonitoredSystem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string[] Dependencies { get; set; }
        }

        public class TelemetryDataPoint
        {
            public string SystemId { get; set; }
            public DateTime Timestamp { get; set; }
            public string MetricName { get; set; }
            public double Value { get; set; }
            public Dictionary<string, string> Tags { get; set; }
        }

        public class HealthIndicator
        {
            public string Name { get; set; }
            public string MetricSource { get; set; }
            public double WarningThreshold { get; set; }
            public double CriticalThreshold { get; set; }
        }

        public class AlertThreshold
        {
            public string Indicator { get; set; }
            public string Level { get; set; } // "warning", "critical"
            public int ConsecutiveOccurrences { get; set; }
            public TimeSpan TimeWindow { get; set; }
        }
    
}
