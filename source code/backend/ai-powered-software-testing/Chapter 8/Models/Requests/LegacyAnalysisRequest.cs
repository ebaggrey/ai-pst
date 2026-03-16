namespace Chapter_8.Models.Requests
{
    // Models/Requests/LegacyAnalysisRequest.cs
   
        public class LegacyAnalysisRequest
        {
            public CodebaseInfo Codebase { get; set; }
            public string AnalysisDepth { get; set; } // "quick", "standard", "comprehensive"
            public BusinessContext BusinessContext { get; set; }
            public string SafetyLevel { get; set; } // "conservative", "balanced", "aggressive"
            public string[] FocusAreas { get; set; }
        }

        public class CodebaseInfo
        {
            public string Name { get; set; }
            public string[] TechnologyStack { get; set; }
            public int AgeYears { get; set; }
            public long TotalLines { get; set; }
            public int ComplexityScore { get; set; }
            public Dependency[] Dependencies { get; set; }
        }

        public class Dependency
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public bool IsExternal { get; set; }
        }

        public class BusinessContext
        {
            public CriticalFlow[] CriticalFlows { get; set; }
            public string[] StakeholderConcerns { get; set; }
            public Dictionary<string, string> BusinessRules { get; set; }
        }

        public class CriticalFlow
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public int BusinessValue { get; set; }
            public string[] InvolvedSystems { get; set; }
        }
    
}
