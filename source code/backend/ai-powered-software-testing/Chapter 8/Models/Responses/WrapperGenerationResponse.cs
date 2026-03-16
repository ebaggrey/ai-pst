

using Chapter_8.Models.Requests;

namespace Chapter_8.Models.Responses
{
    // Models/Responses/WrapperGenerationResponse.cs
   
        public class WrapperGenerationResponse
        {
            public string WrapperId { get; set; }
            public LegacyModule OriginalModule { get; set; }
            public GeneratedWrapper GeneratedWrapper { get; set; }
            public ValidationTest[] ValidationTests { get; set; }
            public MigrationPlan MigrationPlan { get; set; }
            public SafetyAssessment SafetyAssessment { get; set; }
            public PerformanceImpact PerformanceImpact { get; set; }
            public RollbackStrategy RollbackStrategy { get; set; }
        }

        public class GeneratedWrapper
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Language { get; set; }
            public SafetyFeature[] SafetyFeatures { get; set; }
            public string[] ExposedInterfaces { get; set; }
        }

        public class SafetyFeature
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Configuration { get; set; }
        }

        public class ValidationTest
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string TestCode { get; set; }
            public string ExpectedResult { get; set; }
        }

        public class MigrationPlan
        {
            public string Strategy { get; set; }
            public Phase[] Phases { get; set; }
            public string EstimatedDuration { get; set; }
        }

        public class Phase
        {
            public string Name { get; set; }
            public string[] Steps { get; set; }
            public string SuccessCriteria { get; set; }
        }

        public class SafetyAssessment
        {
            public double SafetyScore { get; set; }
            public string[] CoveredRisks { get; set; }
            public string[] UncoveredRisks { get; set; }
            public string[] Recommendations { get; set; }
        }

        public class PerformanceImpact
        {
            public double LatencyIncrease { get; set; }
            public double MemoryOverhead { get; set; }
            public double ThroughputImpact { get; set; }
            public string[] Bottlenecks { get; set; }
        }

        public class RollbackStrategy
        {
            public string Trigger { get; set; }
            public string[] Steps { get; set; }
            public string EstimatedRevertTime { get; set; }
            public string[] DataPreservationSteps { get; set; }
        }
    
}
