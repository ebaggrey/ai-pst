
using Chapter_7.Data.Entities;
namespace Chapter_7.Data.Repositories
{
    // Data/Repositories/IPipelineRepository.cs
    public interface IPipelineRepository
    {
        // Pipeline CRUD
        Task<Pipeline> GetPipelineByIdAsync(Guid id);
        Task<Pipeline> GetPipelineByPipelineIdAsync(string pipelineId);
        Task<IEnumerable<Pipeline>> GetAllPipelinesAsync(bool includeDeleted = false);
        Task<Pipeline> CreatePipelineAsync(Pipeline pipeline);
        Task<Pipeline> UpdatePipelineAsync(Pipeline pipeline);
        Task<bool> DeletePipelineAsync(Guid id, bool softDelete = true);

        // Pipeline Stages
        Task<IEnumerable<PipelineStage>> GetPipelineStagesAsync(Guid pipelineId);
        Task<PipelineStage> CreatePipelineStageAsync(PipelineStage stage);
        Task<PipelineStage> UpdatePipelineStageAsync(PipelineStage stage);

        // Pipeline Runs
        Task<PipelineRun> GetPipelineRunByIdAsync(Guid id);
        Task<PipelineRun> GetPipelineRunByRunIdAsync(string runId);
        Task<IEnumerable<PipelineRun>> GetPipelineRunsAsync(
            Guid pipelineId,
            int? limit = null,
            string? status = null,
            DateTimeOffset? from = null,
            DateTimeOffset? to = null);
        Task<PipelineRun> CreatePipelineRunAsync(PipelineRun run);
        Task<PipelineRun> UpdatePipelineRunAsync(PipelineRun run);

        // Failures
        Task<IEnumerable<PipelineFailure>> GetFailuresByRunIdAsync(Guid runId);
        Task<PipelineFailure> CreatePipelineFailureAsync(PipelineFailure failure);

        // Optimization Opportunities
        Task<IEnumerable<OptimizationOpportunity>> GetOptimizationOpportunitiesAsync(
            Guid pipelineId,
            string? status = null,
            string? category = null);
        Task<OptimizationOpportunity> CreateOptimizationOpportunityAsync(OptimizationOpportunity opportunity);
        Task<OptimizationOpportunity> UpdateOptimizationOpportunityAsync(OptimizationOpportunity opportunity);

        // Adaptation Events
        Task<IEnumerable<AdaptationEvent>> GetAdaptationEventsAsync(
            Guid pipelineId,
            string? status = null,
            string? changeType = null);
        Task<AdaptationEvent> CreateAdaptationEventAsync(AdaptationEvent adaptationEvent);
        Task<AdaptationEvent> UpdateAdaptationEventAsync(AdaptationEvent adaptationEvent);

        // Predictions
        Task<IEnumerable<PredictionResult>> GetPredictionsByPipelineIdAsync(
            Guid pipelineId,
            string? predictionType = null,
            int? limit = null);
        Task<PredictionResult> CreatePredictionResultAsync(PredictionResult prediction);
        Task<PredictionResult> UpdatePredictionResultAsync(PredictionResult prediction);

        // Prediction Models
        Task<PredictionModel> GetActivePredictionModelAsync(string modelType);

        // Analytics and Statistics
        Task<decimal> GetAverageSuccessRateAsync(Guid pipelineId, int days);
        Task<int> GetPipelineRunCountAsync(Guid pipelineId, string? status = null);
        Task<double> GetAverageDurationAsync(Guid pipelineId, string status = "Success");
        Task<Dictionary<string, int>> GetFailureTypesAsync(Guid pipelineId, int days);
        Task<PipelineStatistics> GetPipelineStatisticsAsync(Guid pipelineId);
        Task<IEnumerable<PipelineRun>> GetSlowPipelineRunsAsync(Guid pipelineId, int thresholdSeconds);
        Task<IEnumerable<OptimizationOpportunity>> GetHighImpactOpportunitiesAsync(decimal minImpact);

        // Bulk Operations
        Task<int> BulkUpdateOptimizationOpportunityStatusAsync(IEnumerable<Guid> opportunityIds, string status);

        // Search
        Task<IEnumerable<Pipeline>> SearchPipelinesAsync(
            string? language = null,
            decimal? minTestCoverage = null,
            string? status = null,
            string? searchTerm = null);
    }

}
