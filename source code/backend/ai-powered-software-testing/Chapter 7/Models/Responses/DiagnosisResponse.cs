using Chapter_7.Models.Requests;

namespace Chapter_7.Models.Responses
{
  
    public class DiagnosisResponse
    {
        public string DiagnosisId { get; set; }
        public ParsedFailure FailureDetails { get; set; }
        public RootCause RootCause { get; set; }
        public double Confidence { get; set; }
        public RemediationStep[] RemediationSteps { get; set; }
        public PreventionStrategy[] PreventionStrategies { get; set; }
        public HistoricalFailure[] SimilarHistoricalFailures { get; set; }
        public ImpactAssessment ImpactAssessment { get; set; }
    }

    public class ParsedFailure
    {
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    public class RootCause
    {
        public string Summary { get; set; }
        public string Component { get; set; }
    }

    public class RemediationStep
    {
        public int StepNumber { get; set; }
        public string Action { get; set; }
    }

    public class PreventionStrategy
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class HistoricalFailure
    {
        public string FailureId { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
