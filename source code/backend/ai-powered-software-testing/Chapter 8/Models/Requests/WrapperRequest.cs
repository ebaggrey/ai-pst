namespace Chapter_8.Models.Requests
{
    // Models/Requests/WrapperRequest.cs
   
        public class WrapperRequest
        {
            public LegacyModule LegacyModule { get; set; }
            public string WrapperType { get; set; } // "facade", "adapter", "proxy", "strangler"
            public string SafetyLevel { get; set; } // "conservative", "balanced", "aggressive"
            public SafetyMeasure[] SafetyMeasures { get; set; }
            public ValidationRequirement[] ValidationRequirements { get; set; }
            public string ModernizationStrategy { get; set; } // "strangler-fig", "branch-by-abstraction", "parallel-run"
        }

        public class LegacyModule
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public string[] ExposedFunctions { get; set; }
            public int ComplexityScore { get; set; }
            public Dictionary<string, string> Configuration { get; set; }
        }

        public class SafetyMeasure
        {
            public string Type { get; set; } // "circuit-breaker", "retry", "timeout", "bulkhead"
            public string Configuration { get; set; }
        }

        public class ValidationRequirement
        {
            public string Name { get; set; }
            public string ValidationType { get; set; } // "input", "output", "state", "integration"
            public bool IsMandatory { get; set; }
        }
    
}
