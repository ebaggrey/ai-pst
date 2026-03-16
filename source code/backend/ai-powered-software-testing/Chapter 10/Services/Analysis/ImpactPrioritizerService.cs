
// Services/Analysis/ImpactPrioritizerService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class ImpactPrioritizerService : IImpactPrioritizer
    {
        private readonly ILogger<ImpactPrioritizerService> _logger;

        public ImpactPrioritizerService(ILogger<ImpactPrioritizerService> logger)
        {
            _logger = logger;
        }

        public async Task<ActionableInsight[]> PrioritizeInsightsByImpactAsync(
            ActionableInsight[] insights,
            InsightContext context)
        {
            _logger.LogInformation("Prioritizing {InsightCount} insights by impact", insights?.Length ?? 0);

            if (insights == null || insights.Length == 0)
                return Array.Empty<ActionableInsight>();

            // Calculate priority score for each insight
            foreach (var insight in insights)
            {
                insight.Priority = CalculatePriorityScore(insight, context);
            }

            // Sort by priority (high to low)
            var prioritized = insights
                .OrderByDescending(i => i.Priority)
                .ThenByDescending(i => i.Impact)
                .ToArray();

            return await Task.FromResult(prioritized);
        }

       
        private string CalculatePriorityScore(ActionableInsight insight, InsightContext context)
        {
            double score = insight.ActionabilityScore * 0.4 + insight.Impact * 0.4;

            // Boost score if aligns with business goals
            if (context?.BusinessGoals != null && insight.AffectedMetrics != null)
            {
                var alignment = insight.AffectedMetrics
                    .Count(m => context.BusinessGoals.Any(g =>
                        m.Contains(g, StringComparison.OrdinalIgnoreCase)));
                score += alignment * 0.1;
            }

            // Cap at 1.0
            score = Math.Min(1.0, score);

            return score switch
            {
                > 0.8 => "high",
                > 0.6 => "medium",
                _ => "low"
            };
        }
    }
}
