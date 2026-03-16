
// Services/Analysis/ActionabilityFilterService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class ActionabilityFilterService : IActionabilityFilter
    {
        private readonly ILogger<ActionabilityFilterService> _logger;

        public ActionabilityFilterService(ILogger<ActionabilityFilterService> logger)
        {
            _logger = logger;
        }

        public async Task<ActionableInsight[]> FilterForActionabilityAsync(
            RawInsight[] insights,
            double threshold,
            InsightContext context)
        {
            _logger.LogInformation("Filtering {InsightCount} insights with threshold {Threshold}",
                insights?.Length ?? 0, threshold);

            if (insights == null || insights.Length == 0)
                return Array.Empty<ActionableInsight>();

            var actionableInsights = new List<ActionableInsight>();

            foreach (var insight in insights)
            {
                double actionabilityScore = CalculateActionability(insight, context);

                if (actionabilityScore >= threshold)
                {
                    actionableInsights.Add(new ActionableInsight
                    {
                        InsightId = Guid.NewGuid().ToString(),
                        Title = insight.Title,
                        Description = insight.Description,
                        Type = insight.Type,
                        ActionabilityScore = actionabilityScore,
                        Impact = CalculateImpact(insight),
                        AffectedMetrics = insight.AffectedMetrics,
                        RecommendedActions = GenerateRecommendedActions(insight),
                        Priority = DeterminePriority(actionabilityScore, insight.Confidence)
                    });
                }
            }

            return await Task.FromResult(actionableInsights.ToArray());
        }

        private double CalculateActionability(RawInsight insight, InsightContext context)
        {
            double score = insight.Confidence;

            // Adjust based on context
            if (context?.PriorityLevel == "high")
                score *= 1.1;

            // Adjust based on insight type
            score *= insight.Type?.ToLower() switch
            {
                "anomaly" => 1.2,
                "trend" => 1.1,
                "correlation" => 0.9,
                _ => 1.0
            };

            return Math.Min(1.0, score);
        }

        private double CalculateImpact(RawInsight insight)
        {
            return insight.Type?.ToLower() switch
            {
                "anomaly" => 0.8,
                "trend" => 0.7,
                "correlation" => 0.6,
                "opportunity" => 0.9,
                _ => 0.5
            };
        }

        private string[] GenerateRecommendedActions(RawInsight insight)
        {
            return insight.Type?.ToLower() switch
            {
                "anomaly" => new[] { "Investigate root cause", "Implement monitoring", "Create incident report" },
                "trend" => new[] { "Analyze contributing factors", "Project future impact", "Plan interventions" },
                "correlation" => new[] { "Validate causation", "Identify leverage points", "Test interventions" },
                "opportunity" => new[] { "Create action plan", "Assign ownership", "Set success metrics" },
                _ => new[] { "Review insight", "Discuss with team", "Document findings" }
            };
        }

        private string DeterminePriority(double actionability, double confidence)
        {
            double combined = (actionability + confidence) / 2;
            return combined switch
            {
                > 0.8 => "high",
                > 0.6 => "medium",
                _ => "low"
            };
        }

        Task<ActionableInsight[]> IActionabilityFilter.FilterForActionabilityAsync(RawInsight[] insights, double threshold, InsightContext context)
        {
            throw new NotImplementedException();
        }
    }
}