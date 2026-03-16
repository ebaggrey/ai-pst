namespace Chapter_8.Models.Requests
{
    // Models/Requests/CharacterizationRequest.cs
    
        public class CharacterizationRequest
        {
            public ObservedOutput[] ObservedOutputs { get; set; }
            public LegacyBehavior LegacyBehavior { get; set; }
            public double CoverageGoal { get; set; } // 0.0 to 1.0
            public bool IncludeEdgeCases { get; set; }
            public string TestStrategy { get; set; } // "record-replay", "property-based", "golden-master"
            public bool GenerateDocumentation { get; set; }
        }

        public class ObservedOutput
        {
            public string Input { get; set; }
            public string Output { get; set; }
            public DateTime Timestamp { get; set; }
            public string Context { get; set; }
        }

        public class LegacyBehavior
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string[] KnownVariations { get; set; }
        }
    
}
