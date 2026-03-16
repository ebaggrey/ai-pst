namespace Chapter_7.Models.Errors
{
    
    public class PipelineErrorResponse
    {
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public string[] RecoverySteps { get; set; }
        public string FallbackSuggestion { get; set; }
    }
}
