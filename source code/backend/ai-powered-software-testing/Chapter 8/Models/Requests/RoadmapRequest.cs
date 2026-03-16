namespace Chapter_8.Models.Requests
{
    // Models/Requests/RoadmapRequest.cs
   
        public class RoadmapRequest
        {
            public LegacyAnalysis LegacyAnalysis { get; set; }
            public BusinessPriority[] BusinessPriorities { get; set; }
            public string ModernizationApproach { get; set; } // "big-bang", "incremental", "strangler"
            public PipelineConstraints Constraints { get; set; }
            public SuccessMetric[] SuccessMetrics { get; set; }
        }

        public class BusinessPriority
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Priority { get; set; } // 1-10
            public string[] DependentSystems { get; set; }
        }

        public class PipelineConstraints
        {
            public string MaxDuration { get; set; }
            public decimal MaxCostPerRun { get; set; }
            public int MaxParallelWorkers { get; set; }
            public string[] AllowedDowntimeWindows { get; set; }

            public int GetConstraintCount()
            {
                return (MaxDuration != null ? 1 : 0) +
                       (MaxCostPerRun > 0 ? 1 : 0) +
                       (MaxParallelWorkers > 0 ? 1 : 0) +
                       (AllowedDowntimeWindows?.Length > 0 ? 1 : 0);
            }
        }

        public class SuccessMetric
        {
            public string Name { get; set; }
            public string TargetValue { get; set; }
            public string MeasurementMethod { get; set; }
        }
    
}
