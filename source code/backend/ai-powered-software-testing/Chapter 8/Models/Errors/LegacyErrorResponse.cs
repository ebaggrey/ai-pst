namespace Chapter_8.Models.Errors
{
    // Models/Errors/LegacyErrorResponse.cs
    
        public class LegacyErrorResponse
        {
            public string ErrorType { get; set; }
            public string Message { get; set; }
            public string[] RecoverySteps { get; set; }
            public string FallbackSuggestion { get; set; }
            public LegacyDiagnosticData DiagnosticData { get; set; }
        }

        public class LegacyDiagnosticData
        {
            // For complexity exceptions
            public double? ProblematicComplexity { get; set; }
            public string SuggestedSimplification { get; set; }

            // For wrapper exceptions
            public string ProblematicModule { get; set; }
            public string[] ComplexityIssues { get; set; }

            // For behavior ambiguity
            public string[] AmbiguityAreas { get; set; }
            public string[] SuggestedClarifications { get; set; }

            // For constraint violations
            public string[] ConflictingConstraints { get; set; }
            public string[] SuggestedAdjustments { get; set; }

            // For telemetry inconsistencies
            public string InconsistencyDetails { get; set; }
            public string[] DataQualityIssues { get; set; }
        }
    
}
