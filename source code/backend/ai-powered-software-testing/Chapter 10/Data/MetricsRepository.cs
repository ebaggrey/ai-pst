
// Data/MetricsRepository.cs
using Chapter_10.Data;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Microsoft.EntityFrameworkCore;


namespace Chapter_10.Data
{
    public class MetricsRepository : IMetricsRepository
    {
        private readonly MetricsDbContext _context;
        private readonly ILogger<MetricsRepository> _logger;

        public MetricsRepository(
            MetricsDbContext context,
            ILogger<MetricsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MetricDesignEntity> SaveMetricDesignAsync(MetricDesignEntity design)
        {
            try
            {
                _context.MetricDesigns.Add(design);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved metric design {DesignId}", design.DesignId);
                return design;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving metric design");
                throw;
            }
        }

        public async Task<MetricDesignEntity?> GetMetricDesignAsync(string designId)
        {
            return await _context.MetricDesigns
                .FirstOrDefaultAsync(d => d.DesignId == designId && d.IsActive);
        }

        public async Task<List<MetricDesignEntity>> GetMetricDesignsAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.MetricDesigns.Where(d => d.IsActive);

            if (from.HasValue)
                query = query.Where(d => d.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(d => d.CreatedAt <= to.Value);

            return await query.OrderByDescending(d => d.CreatedAt).ToListAsync();
        }

        public async Task<HealthScoreEntity> SaveHealthScoreAsync(HealthScoreEntity healthScore)
        {
            try
            {
                _context.HealthScores.Add(healthScore);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved health score {HealthScoreId}", healthScore.HealthScoreId);
                return healthScore;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving health score");
                throw;
            }
        }

        public async Task<HealthScoreEntity?> GetLatestHealthScoreAsync(string? metricId = null)
        {
            var query = _context.HealthScores.AsQueryable();

            if (!string.IsNullOrEmpty(metricId))
            {
                // Filter by metric if needed - would need to parse JSON
                // This is simplified - in production, consider a different schema
            }

            return await query.OrderByDescending(h => h.CalculatedAt).FirstOrDefaultAsync();
        }

        public async Task<List<HealthScoreEntity>> GetHealthScoreHistoryAsync(string metricId, int days)
        {
            var from = DateTime.UtcNow.AddDays(-days);

            // Simplified - in production, would need to filter by metricId in JSON
            return await _context.HealthScores
                .Where(h => h.CalculatedAt >= from)
                .OrderByDescending(h => h.CalculatedAt)
                .ToListAsync();
        }

        public async Task<PredictionEntity> SavePredictionAsync(PredictionEntity prediction)
        {
            try
            {
                _context.Predictions.Add(prediction);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved prediction {PredictionId}", prediction.PredictionId);
                return prediction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving prediction");
                throw;
            }
        }

        public async Task<PredictionEntity?> GetPredictionAsync(string predictionId)
        {
            return await _context.Predictions
                .FirstOrDefaultAsync(p => p.PredictionId == predictionId);
        }

        public async Task<List<PredictionEntity>> GetActivePredictionsAsync()
        {
            return await _context.Predictions
                .Where(p => p.ValidUntil >= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<InsightEntity> SaveInsightAsync(InsightEntity insight)
        {
            try
            {
                _context.Insights.Add(insight);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved insight {InsightId}", insight.InsightId);
                return insight;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving insight");
                throw;
            }
        }

        public async Task<List<InsightEntity>> GetInsightsAsync(bool onlyActionable = true)
        {
            var query = _context.Insights.AsQueryable();

            if (onlyActionable)
                query = query.Where(i => i.ActionabilityScore >= 0.7);

            return await query.OrderByDescending(i => i.GeneratedAt).ToListAsync();
        }

        public async Task UpdateInsightStatusAsync(string insightId, bool isImplemented)
        {
            var insight = await _context.Insights.FirstOrDefaultAsync(i => i.InsightId == insightId);
            if (insight != null)
            {
                insight.IsImplemented = isImplemented;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<OptimizationEntity> SaveOptimizationAsync(OptimizationEntity optimization)
        {
            try
            {
                _context.Optimizations.Add(optimization);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved optimization {OptimizationId}", optimization.OptimizationId);
                return optimization;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving optimization");
                throw;
            }
        }

        public async Task<List<OptimizationEntity>> GetPendingOptimizationsAsync()
        {
            return await _context.Optimizations
                .Where(o => o.Status == "pending" && o.ImplementedAt == null)
                .ToListAsync();
        }

        public async Task<MetricDefinitionEntity> SaveMetricDefinitionAsync(MetricDefinitionEntity definition)
        {
            try
            {
                var existing = await GetMetricDefinitionAsync(definition.MetricId);
                if (existing != null)
                {
                    existing.Name = definition.Name;
                    existing.Category = definition.Category;
                    existing.CollectionMethod = definition.CollectionMethod;
                    existing.CollectionCost = definition.CollectionCost;
                    existing.BusinessValue = definition.BusinessValue;
                    existing.TargetValue = definition.TargetValue;
                    existing.IsActive = definition.IsActive;
                }
                else
                {
                    _context.MetricDefinitions.Add(definition);
                }

                await _context.SaveChangesAsync();
                return definition;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving metric definition");
                throw;
            }
        }

        public async Task<MetricDefinitionEntity?> GetMetricDefinitionAsync(string metricId)
        {
            return await _context.MetricDefinitions
                .FirstOrDefaultAsync(m => m.MetricId == metricId);
        }

        public async Task<List<MetricDefinitionEntity>> GetAllMetricDefinitionsAsync(bool onlyActive = true)
        {
            var query = _context.MetricDefinitions.AsQueryable();

            if (onlyActive)
                query = query.Where(m => m.IsActive);

            return await query.ToListAsync();
        }

        public async Task<HistoricalDataEntity> SaveHistoricalDataAsync(HistoricalDataEntity data)
        {
            try
            {
                _context.HistoricalData.Add(data);
                await _context.SaveChangesAsync();
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving historical data");
                throw;
            }
        }

        public async Task<List<HistoricalDataEntity>> GetHistoricalDataAsync(string metricId, DateTime from, DateTime to)
        {
            return await _context.HistoricalData
                .Where(h => h.MetricId == metricId && h.Timestamp >= from && h.Timestamp <= to)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }

        public async Task<HistoricalBaseline?> CalculateBaselineAsync(string metricId, DateTime from, DateTime to)
        {
            var data = await GetHistoricalDataAsync(metricId, from, to);

            if (!data.Any())
                return null;

            var values = data.Select(d => d.Value).ToList();

            return new HistoricalBaseline
            {
                MetricId = metricId,
                Mean = values.Average(),
                StandardDeviation = CalculateStdDev(values),
                Min = values.Min(),
                Max = values.Max(),
                SampleSize = values.Count,
                PeriodStart = from,
                PeriodEnd = to
            };
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private double CalculateStdDev(List<double> values)
        {
            if (values.Count < 2) return 0;

            var avg = values.Average();
            var sum = values.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sum / (values.Count - 1));
        }
    }
}