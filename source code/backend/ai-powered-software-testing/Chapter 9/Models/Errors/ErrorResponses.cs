namespace Chapter_9.Models.Errors
{
    // Models/Errors/LeanErrorResponse.cs
    public class LeanErrorResponse
    {
        public string Context { get; set; }
        public string ErrorType { get; set; }
        public string LeanPrincipleViolated { get; set; }
        public string Message { get; set; }
        public string[] RecoverySteps { get; set; }
        public string FallbackSuggestion { get; set; }
        public LeanDiagnosticData DiagnosticData { get; set; }
    }

    public class LeanDiagnosticData
    {
        public object ConstraintAnalysis { get; set; }
        public object SuggestedAdjustments { get; set; }
        public object CoverageGapAnalysis { get; set; }
        public object ConstraintImpact { get; set; }
        public object MissingCostData { get; set; }
        public object EstimationChallenges { get; set; }
        public object ViolatedRules { get; set; }
        public object AffectedTests { get; set; }
        public object DataGaps { get; set; }
        public object CalculationLimitations { get; set; }
    }
}



