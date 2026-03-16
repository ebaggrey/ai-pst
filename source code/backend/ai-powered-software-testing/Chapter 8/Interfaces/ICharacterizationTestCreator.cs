using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;

namespace Chapter_8.Interfaces
{
    // Interfaces/ICharacterizationTestCreator.cs
  
        public interface ICharacterizationTestCreator
        {
            Task<CharacterizationTest[]> CreateCharacterizationTestsAsync(
                BehaviorAnalysis behaviorAnalysis,
                LegacyBehavior legacyBehavior,
                double coverageGoal,
                bool includeEdgeCases);
        }

        public class BehaviorAnalysis
        {
            public string BehaviorId { get; set; }
            public ObservedPattern[] ObservedPatterns { get; set; }
            public InputOutputSpace IOSpace { get; set; }
            public string[] EdgeCases { get; set; }
            public double BehaviorStability { get; set; }
        }

        public class ObservedPattern
        {
            public string PatternType { get; set; }
            public string Description { get; set; }
            public int OccurrenceCount { get; set; }
            public double Confidence { get; set; }
        }

        public class InputOutputSpace
        {
            public string[] InputDomains { get; set; }
            public string[] OutputRanges { get; set; }
            public Dictionary<string, string> Mappings { get; set; }
        }
    
}
