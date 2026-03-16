//namespace Chapter_11.Interfaces
//{
//    public class ISpectrumPipelineBuilder
//    {
//    }
//}

// Interfaces/ISpectrumPipelineBuilder.cs
using Chapter_11.Models.Analysis;
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;


namespace Chapter_11.Interfaces
{
    public interface ISpectrumPipelineBuilder
    {
        Task<Pipeline> BuildPipelineAsync(
            StageAnalysis stageAnalysis,
            GateMapping gateMapping,
            SpectrumCoverage coverage,
            FeedbackMechanism[] feedbackMechanisms);
    }
}
