
// Interfaces/IMetricsRepository.cs
using Chapter_10.Data;
using Chapter_10.Models.Requests;

namespace Chapter_10.Interfaces
{
    public interface IMetricsRepository
    {
        // Metric Design
        Task<MetricDesignEntity> SaveMetricDesignAsync(MetricDesignEntity design);
        Task<MetricDesignEntity?> GetMetricDesignAsync(string designId);
        Task<List<MetricDesignEntity>> GetMetricDesignsAsync(DateTime? from = null, DateTime? to = null);

        // Health Scores
        Task<HealthScoreEntity> SaveHealthScoreAsync(HealthScoreEntity healthScore);
        Task<HealthScoreEntity?> GetLatestHealthScoreAsync(string? metricId = null);
        Task<List<HealthScoreEntity>> GetHealthScoreHistoryAsync(string metricId, int days);

        // Predictions
        Task<PredictionEntity> SavePredictionAsync(PredictionEntity prediction);
        Task<PredictionEntity?> GetPredictionAsync(string predictionId);
        Task<List<PredictionEntity>> GetActivePredictionsAsync();

        // Insights
        Task<InsightEntity> SaveInsightAsync(InsightEntity insight);
        Task<List<InsightEntity>> GetInsightsAsync(bool onlyActionable = true);
        Task UpdateInsightStatusAsync(string insightId, bool isImplemented);

        // Optimizations
        Task<OptimizationEntity> SaveOptimizationAsync(OptimizationEntity optimization);
        Task<List<OptimizationEntity>> GetPendingOptimizationsAsync();

        // Metric Definitions
        Task<MetricDefinitionEntity> SaveMetricDefinitionAsync(MetricDefinitionEntity definition);
        Task<MetricDefinitionEntity?> GetMetricDefinitionAsync(string metricId);
        Task<List<MetricDefinitionEntity>> GetAllMetricDefinitionsAsync(bool onlyActive = true);

        // Historical Data
        Task<HistoricalDataEntity> SaveHistoricalDataAsync(HistoricalDataEntity data);
        Task<List<HistoricalDataEntity>> GetHistoricalDataAsync(string metricId, DateTime from, DateTime to);
        Task<HistoricalBaseline?> CalculateBaselineAsync(string metricId, DateTime from, DateTime to);

        // Bulk Operations
        Task SaveChangesAsync();
    }
}
