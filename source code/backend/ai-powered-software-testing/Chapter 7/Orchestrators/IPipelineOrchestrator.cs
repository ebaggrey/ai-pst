using Chapter_7.Exceptions;
using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;

namespace Chapter_7.Orchestrators
{
  
    public interface IPipelineOrchestrator
    {
        Task<IntelligentPipelineResponse> GeneratePipelineAsync(PipelineGenerationRequest request);
        Task<IntelligentPipelineResponse> HandleConstraintConflictAsync(
            ConstraintConflictException ex,
            PipelineGenerationRequest request);
    }

    public interface IDiagnosisOrchestrator
    {
        Task<DiagnosisResponse> DiagnoseFailureAsync(DiagnosisRequest request);
    }

    public interface IOptimizationOrchestrator
    {
        Task<OptimizationResponse> OptimizePerformanceAsync(OptimizationRequest request);
    }

    public interface IPredictionOrchestrator
    {
        Task<PredictionResponse> PredictIssuesAsync(PredictionRequest request);
    }

    public interface IAdaptationOrchestrator
    {
        Task<AdaptationResponse> AdaptPipelineAsync(AdaptationRequest request);
    }
}
