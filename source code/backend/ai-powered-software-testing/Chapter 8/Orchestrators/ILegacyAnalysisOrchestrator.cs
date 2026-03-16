
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;

namespace Chapter_8.Orchestrators
{
    public interface ILegacyAnalysisOrchestrator
    {
        Task<LegacyAnalysisResponse> AnalyzeLegacyCodebaseAsync(LegacyAnalysisRequest request);
    }
}
