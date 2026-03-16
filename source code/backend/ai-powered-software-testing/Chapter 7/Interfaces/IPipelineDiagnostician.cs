using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;

namespace Chapter_7.Interfaces
{
   
    public interface IPipelineDiagnostician
    {
        Task<RootCause> IdentifyRootCauseAsync(
            ParsedFailure parsedFailure,
            ChangeCorrelation changeCorrelation,
            DiagnosisDepth diagnosisDepth);
    }

    public class ChangeCorrelation
    {
        public Change[] RelatedChanges { get; set; }
        public double CorrelationScore { get; set; }
    }
}
