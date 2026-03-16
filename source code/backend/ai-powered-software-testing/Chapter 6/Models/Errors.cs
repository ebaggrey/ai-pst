namespace Chapter_6.Models
{
   
        public class BDDErrorResponse
        {
            public string ErrorType { get; set; } = string.Empty;
            public string Phase { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string[] RecoveryPath { get; set; } = Array.Empty<string>();
            public string FallbackSuggestion { get; set; } = string.Empty;
            public BDDConflictDetails ConflictDetails { get; set; } = new BDDConflictDetails();
        }

        public class BDDConflictDetails
        {
            public string[] ConflictingScenarios { get; set; } = Array.Empty<string>();
            public string[] AmbiguityAreas { get; set; } = Array.Empty<string>();
            public string[] StakeholderConflicts { get; set; } = Array.Empty<string>();
        }
    
}
