
// Models/Error/SpectrumErrorResponse.cs
namespace FullSpectrumApp.Models.Error
{
    public class SpectrumErrorResponse
    {
        public string Context { get; set; }
        public string ErrorType { get; set; }
        public string SpectrumLocation { get; set; }
        public string Message { get; set; }
        public string[] RecoverySteps { get; set; }
        public string FallbackSuggestion { get; set; }
        public SpectrumDiagnosticData DiagnosticData { get; set; }
    }

    public class SpectrumDiagnosticData
    {
        public string[] AmbiguousRequirements { get; set; }
        public string[] ClarificationQuestions { get; set; }
        public string[] FrameworkIssues { get; set; }
        public string[] TechnologyMismatches { get; set; }
        public string[] ComplexityIssues { get; set; }
        public string[] RecommendedSimplifications { get; set; }
        public string[] ConflictingStages { get; set; }
        public string[] DependencyIssues { get; set; }
        public string[] ComplexityFactors { get; set; }
        public string[] SimplificationSuggestions { get; set; }
    }
}
