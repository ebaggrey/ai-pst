using Chapter_7.Data.Context;
using Chapter_7.Data.Entities;
using System.Data.Entity;

namespace Chapter_7.Data.Repositories
{
    
    public class PipelineRepository : IPipelineRepository
    {
        private readonly PipelineDbContext _context;
        private readonly ILogger<PipelineRepository> _logger;

        public PipelineRepository(
            PipelineDbContext context,
            ILogger<PipelineRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Pipeline CRUD Operations
        public async Task<Pipeline> GetPipelineByIdAsync(Guid id)
        {
            try
            {
                return await _context.Pipelines
                    .Include(p => p.Stages)
                    .Include(p => p.OptimizationOpportunities)
                    .Include(p => p.AdaptationEvents)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline by ID: {PipelineId}", id);
                throw;
            }
        }

        public async Task<Pipeline> GetPipelineByPipelineIdAsync(string pipelineId)
        {
            try
            {
                return await _context.Pipelines
                    .Include(p => p.Stages)
                    .Include(p => p.Runs.Take(10)) // Last 10 runs
                    .FirstOrDefaultAsync(p => p.PipelineId == pipelineId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline by PipelineId: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<IEnumerable<Pipeline>> GetAllPipelinesAsync(bool includeDeleted = false)
        {
            try
            {
                var query = _context.Pipelines
                    .Include(p => p.Stages)
                    .AsQueryable();

                if (!includeDeleted)
                {
                    query = query.Where(p => !p.IsDeleted);
                }

                return await query
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all pipelines");
                throw;
            }
        }

        public async Task<Pipeline> CreatePipelineAsync(Pipeline pipeline)
        {
            try
            {
                pipeline.Id = Guid.NewGuid();
                pipeline.CreatedAt = DateTimeOffset.UtcNow;
                pipeline.Status = "Active";

                await _context.Pipelines.AddAsync(pipeline);
                await _context.SaveChangesAsync();

                return pipeline;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pipeline: {PipelineName}", pipeline.Name);
                throw;
            }
        }

        public async Task<Pipeline> UpdatePipelineAsync(Pipeline pipeline)
        {
            try
            {
                pipeline.ModifiedAt = DateTimeOffset.UtcNow;
                _context.Pipelines.Update(pipeline);
                await _context.SaveChangesAsync();

                return pipeline;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pipeline: {PipelineId}", pipeline.PipelineId);
                throw;
            }
        }

        public async Task<bool> DeletePipelineAsync(Guid id, bool softDelete = true)
        {
            try
            {
                var pipeline = await _context.Pipelines.FindAsync(id);

                if (pipeline == null)
                    return false;

                if (softDelete)
                {
                    pipeline.IsDeleted = true;
                    pipeline.ModifiedAt = DateTimeOffset.UtcNow;
                    _context.Pipelines.Update(pipeline);
                }
                else
                {
                    _context.Pipelines.Remove(pipeline);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pipeline: {PipelineId}", id);
                throw;
            }
        }

        // Pipeline Stages
        public async Task<IEnumerable<PipelineStage>> GetPipelineStagesAsync(Guid pipelineId)
        {
            try
            {
                return await _context.PipelineStages
                    .Where(s => s.PipelineId == pipelineId)
                    .OrderBy(s => s.StageOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stages for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<PipelineStage> CreatePipelineStageAsync(PipelineStage stage)
        {
            try
            {
                stage.Id = Guid.NewGuid();
                stage.CreatedAt = DateTimeOffset.UtcNow;

                await _context.PipelineStages.AddAsync(stage);
                await _context.SaveChangesAsync();

                return stage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pipeline stage: {StageName}", stage.StageName);
                throw;
            }
        }

        public async Task<PipelineStage> UpdatePipelineStageAsync(PipelineStage stage)
        {
            try
            {
                stage.ModifiedAt = DateTimeOffset.UtcNow;
                _context.PipelineStages.Update(stage);
                await _context.SaveChangesAsync();

                return stage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pipeline stage: {StageId}", stage.StageId);
                throw;
            }
        }

        // Pipeline Runs
        public async Task<PipelineRun> GetPipelineRunByIdAsync(Guid id)
        {
            try
            {
                return await _context.PipelineRuns
                    .Include(r => r.Failures)
                    .Include(r => r.Pipeline)
                    .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline run by ID: {RunId}", id);
                throw;
            }
        }

        public async Task<PipelineRun> GetPipelineRunByRunIdAsync(string runId)
        {
            try
            {
                return await _context.PipelineRuns
                    .Include(r => r.Failures)
                    .FirstOrDefaultAsync(r => r.RunId == runId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline run by RunId: {RunId}", runId);
                throw;
            }
        }

        public async Task<IEnumerable<PipelineRun>> GetPipelineRunsAsync(
            Guid pipelineId,
            int? limit = null,
            string? status = null,
            DateTimeOffset? from = null,
            DateTimeOffset? to = null)
        {
            try
            {
                var query = _context.PipelineRuns
                    .Where(r => r.PipelineId == pipelineId)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(status))
                    query = query.Where(r => r.Status == status);

                if (from.HasValue)
                    query = query.Where(r => r.TriggeredAt >= from.Value);

                if (to.HasValue)
                    query = query.Where(r => r.TriggeredAt <= to.Value);

                query = query.OrderByDescending(r => r.TriggeredAt);

                if (limit.HasValue)
                    query = query.Take(limit.Value);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting runs for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<PipelineRun> CreatePipelineRunAsync(PipelineRun run)
        {
            try
            {
                run.Id = Guid.NewGuid();
                run.CreatedAt = DateTimeOffset.UtcNow;

                await _context.PipelineRuns.AddAsync(run);
                await _context.SaveChangesAsync();

                return run;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pipeline run: {RunId}", run.RunId);
                throw;
            }
        }

        public async Task<PipelineRun> UpdatePipelineRunAsync(PipelineRun run)
        {
            try
            {
                _context.PipelineRuns.Update(run);
                await _context.SaveChangesAsync();

                return run;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pipeline run: {RunId}", run.RunId);
                throw;
            }
        }

        // Failures
        public async Task<IEnumerable<PipelineFailure>> GetFailuresByRunIdAsync(Guid runId)
        {
            try
            {
                return await _context.PipelineFailures
                    .Where(f => f.RunId == runId)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting failures for run: {RunId}", runId);
                throw;
            }
        }

        public async Task<PipelineFailure> CreatePipelineFailureAsync(PipelineFailure failure)
        {
            try
            {
                failure.Id = Guid.NewGuid();
                failure.CreatedAt = DateTimeOffset.UtcNow;

                await _context.PipelineFailures.AddAsync(failure);
                await _context.SaveChangesAsync();

                return failure;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pipeline failure: {FailureType}", failure.FailureType);
                throw;
            }
        }

        // Optimization Opportunities
        public async Task<IEnumerable<OptimizationOpportunity>> GetOptimizationOpportunitiesAsync(
            Guid pipelineId,
            string? status = null,
            string? category = null)
        {
            try
            {
                var query = _context.OptimizationOpportunities
                    .Where(o => o.PipelineId == pipelineId)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(status))
                    query = query.Where(o => o.Status == status);

                if (!string.IsNullOrWhiteSpace(category))
                    query = query.Where(o => o.Category == category);

                return await query
                    .OrderByDescending(o => o.Impact)
                    .ThenBy(o => o.Effort)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting optimization opportunities for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<OptimizationOpportunity> CreateOptimizationOpportunityAsync(OptimizationOpportunity opportunity)
        {
            try
            {
                opportunity.Id = Guid.NewGuid();
                opportunity.CreatedAt = DateTimeOffset.UtcNow;
                opportunity.Status = "Identified";

                await _context.OptimizationOpportunities.AddAsync(opportunity);
                await _context.SaveChangesAsync();

                return opportunity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating optimization opportunity: {OpportunityId}", opportunity.OpportunityId);
                throw;
            }
        }

        public async Task<OptimizationOpportunity> UpdateOptimizationOpportunityAsync(OptimizationOpportunity opportunity)
        {
            try
            {
                if (opportunity.Status == "Implemented" && !opportunity.ImplementedAt.HasValue)
                    opportunity.ImplementedAt = DateTimeOffset.UtcNow;

                _context.OptimizationOpportunities.Update(opportunity);
                await _context.SaveChangesAsync();

                return opportunity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating optimization opportunity: {OpportunityId}", opportunity.OpportunityId);
                throw;
            }
        }

        // Adaptation Events
        public async Task<IEnumerable<AdaptationEvent>> GetAdaptationEventsAsync(
            Guid pipelineId,
            string? status = null,
            string? changeType = null)
        {
            try
            {
                var query = _context.AdaptationEvents
                    .Where(a => a.PipelineId == pipelineId)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(status))
                    query = query.Where(a => a.Status == status);

                if (!string.IsNullOrWhiteSpace(changeType))
                    query = query.Where(a => a.ChangeType == changeType);

                return await query
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting adaptation events for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<AdaptationEvent> CreateAdaptationEventAsync(AdaptationEvent adaptationEvent)
        {
            try
            {
                adaptationEvent.Id = Guid.NewGuid();
                adaptationEvent.CreatedAt = DateTimeOffset.UtcNow;
                adaptationEvent.Status = "Proposed";

                await _context.AdaptationEvents.AddAsync(adaptationEvent);
                await _context.SaveChangesAsync();

                return adaptationEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating adaptation event: {AdaptationId}", adaptationEvent.AdaptationId);
                throw;
            }
        }

        public async Task<AdaptationEvent> UpdateAdaptationEventAsync(AdaptationEvent adaptationEvent)
        {
            try
            {
                if (adaptationEvent.Status == "Implemented" && !adaptationEvent.ImplementedAt.HasValue)
                    adaptationEvent.ImplementedAt = DateTimeOffset.UtcNow;

                _context.AdaptationEvents.Update(adaptationEvent);
                await _context.SaveChangesAsync();

                return adaptationEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating adaptation event: {AdaptationId}", adaptationEvent.AdaptationId);
                throw;
            }
        }

        // Predictions
        public async Task<IEnumerable<PredictionResult>> GetPredictionsByPipelineIdAsync(
            Guid pipelineId,
            string? predictionType = null,
            int? limit = null)
        {
            try
            {
                var query = _context.PredictionResults
                    .Include(p => p.Model)
                    .Where(p => p.PipelineId == pipelineId)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(predictionType))
                    query = query.Where(p => p.PredictionType == predictionType);

                query = query.OrderByDescending(p => p.CreatedAt);

                if (limit.HasValue)
                    query = query.Take(limit.Value);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting predictions for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<PredictionResult> CreatePredictionResultAsync(PredictionResult prediction)
        {
            try
            {
                prediction.Id = Guid.NewGuid();
                prediction.CreatedAt = DateTimeOffset.UtcNow;

                await _context.PredictionResults.AddAsync(prediction);
                await _context.SaveChangesAsync();

                return prediction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prediction result: {PredictionId}", prediction.PredictionId);
                throw;
            }
        }

        public async Task<PredictionResult> UpdatePredictionResultAsync(PredictionResult prediction)
        {
            try
            {
                if (prediction.ActualOutcome != null && !prediction.ValidatedAt.HasValue)
                    prediction.ValidatedAt = DateTimeOffset.UtcNow;

                _context.PredictionResults.Update(prediction);
                await _context.SaveChangesAsync();

                return prediction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating prediction result: {PredictionId}", prediction.PredictionId);
                throw;
            }
        }

        // Prediction Models
        public async Task<PredictionModel> GetActivePredictionModelAsync(string modelType)
        {
            try
            {
                return await _context.PredictionModels
                    .Where(m => m.ModelType == modelType && m.IsActive)
                    .OrderByDescending(m => m.Version)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active prediction model: {ModelType}", modelType);
                throw;
            }
        }

        // Analytics and Statistics (Using EF Core LINQ)
        public async Task<decimal> GetAverageSuccessRateAsync(Guid pipelineId, int days)
        {
            try
            {
                var cutoffDate = DateTimeOffset.UtcNow.AddDays(-days);

                var runs = await _context.PipelineRuns
                    .Where(r => r.PipelineId == pipelineId && r.TriggeredAt >= cutoffDate)
                    .Select(r => r.Status)
                    .ToListAsync();

                if (!runs.Any())
                    return 0;

                var successCount = runs.Count(r => r == "Success");
                return (decimal)successCount / runs.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average success rate for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<int> GetPipelineRunCountAsync(Guid pipelineId, string? status = null)
        {
            try
            {
                var query = _context.PipelineRuns
                    .Where(r => r.PipelineId == pipelineId)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(status))
                    query = query.Where(r => r.Status == status);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline run count for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<double> GetAverageDurationAsync(Guid pipelineId, string status = "Success")
        {
            try
            {
                var durations = await _context.PipelineRuns
                    .Where(r => r.PipelineId == pipelineId && r.Status == status && r.Duration.HasValue)
                    .Select(r => r.Duration.Value)
                    .ToListAsync();

                return durations.Any() ? durations.Average() : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average duration for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetFailureTypesAsync(Guid pipelineId, int days)
        {
            try
            {
                var cutoffDate = DateTimeOffset.UtcNow.AddDays(-days);

                var failures = await _context.PipelineRuns
                    .Where(r => r.PipelineId == pipelineId
                        && r.Status == "Failed"
                        && r.ErrorType != null
                        && r.TriggeredAt >= cutoffDate)
                    .GroupBy(r => r.ErrorType)
                    .Select(g => new { ErrorType = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(g => g.ErrorType ?? "Unknown", g => g.Count);

                return failures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting failure types for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<PipelineStatistics> GetPipelineStatisticsAsync(Guid pipelineId)
        {
            try
            {
                var runs = await _context.PipelineRuns
                    .Where(r => r.PipelineId == pipelineId)
                    .ToListAsync();

                var statistics = new PipelineStatistics
                {
                    TotalRuns = runs.Count,
                    SuccessfulRuns = runs.Count(r => r.Status == "Success"),
                    FailedRuns = runs.Count(r => r.Status == "Failed"),
                    AverageDuration = runs.Where(r => r.Status == "Success" && r.Duration.HasValue)
                                         .Select(r => r.Duration.Value)
                                         .DefaultIfEmpty(0)
                                         .Average(),
                    FirstRun = runs.MinBy(r => r.TriggeredAt)?.TriggeredAt,
                    LastRun = runs.MaxBy(r => r.TriggeredAt)?.TriggeredAt
                };

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline statistics for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<IEnumerable<PipelineRun>> GetSlowPipelineRunsAsync(Guid pipelineId, int thresholdSeconds)
        {
            try
            {
                return await _context.PipelineRuns
                    .Where(r => r.PipelineId == pipelineId
                        && r.Duration.HasValue
                        && r.Duration.Value > thresholdSeconds
                        && r.Status == "Success")
                    .OrderByDescending(r => r.Duration)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting slow pipeline runs for pipeline: {PipelineId}", pipelineId);
                throw;
            }
        }

        public async Task<IEnumerable<OptimizationOpportunity>> GetHighImpactOpportunitiesAsync(decimal minImpact)
        {
            try
            {
                return await _context.OptimizationOpportunities
                    .Include(o => o.Pipeline)
                    .Where(o => o.Impact >= minImpact
                        && (o.Status == "Identified" || o.Status == "InProgress"))
                    .OrderByDescending(o => o.Impact)
                    .ThenBy(o => o.Effort)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting high impact optimization opportunities");
                throw;
            }
        }

        // Bulk Operations
        public async Task<int> BulkUpdateOptimizationOpportunityStatusAsync(
            IEnumerable<Guid> opportunityIds,
            string status)
        {
            try
            {
                var opportunities = await _context.OptimizationOpportunities
                    .Where(o => opportunityIds.Contains(o.Id))
                    .ToListAsync();

                foreach (var opportunity in opportunities)
                {
                    opportunity.Status = status;
                    if (status == "Implemented" && !opportunity.ImplementedAt.HasValue)
                        opportunity.ImplementedAt = DateTimeOffset.UtcNow;
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating optimization opportunities");
                throw;
            }
        }

        // Search
        public async Task<IEnumerable<Pipeline>> SearchPipelinesAsync(
            string? language = null,
            decimal? minTestCoverage = null,
            string? status = null,
            string? searchTerm = null)
        {
            try
            {
                var query = _context.Pipelines
                    .Include(p => p.Stages)
                    .Where(p => !p.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(language))
                    query = query.Where(p => p.Language == language);

                if (minTestCoverage.HasValue)
                    query = query.Where(p => p.TestCoverage >= minTestCoverage.Value);

                if (!string.IsNullOrWhiteSpace(status))
                    query = query.Where(p => p.Status == status);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                    query = query.Where(p =>
                        p.Name.Contains(searchTerm) ||
                        (p.Description != null && p.Description.Contains(searchTerm)));

                return await query
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching pipelines");
                throw;
            }
        }
    }
}
