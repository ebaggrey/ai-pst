
// Services/Analysis/InsightQualityAssessorService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class InsightQualityAssessorService : IInsightQualityAssessor
    {
        private readonly ILogger<InsightQualityAssessorService> _logger;

        public InsightQualityAssessorService(ILogger<InsightQualityAssessorService> logger)
        {
            _logger = logger;
        }

        public async Task<InsightQuality> AssessInsightQualityAsync(
            ActionableInsight[] insights,
            MetricValue[] metrics)
        {
            _logger.LogInformation("Assessing quality of {InsightCount} insights", insights?.Length ?? 0);

            if (insights == null || insights.Length == 0)
            {
                return new InsightQuality
                {
                    Accuracy = 0,
                    Relevance = 0,
                    Timeliness = 0,
                    Limitations = new[] { "No insights to assess" }
                };
            }

            double accuracy = CalculateAccuracy(insights);
            double relevance = CalculateRelevance(insights);
            double timeliness = CalculateTimeliness(insights, metrics);
            var limitations = IdentifyLimitations(insights);

            return await Task.FromResult(new InsightQuality
            {
                Accuracy = accuracy,
                Relevance = relevance,
                Timeliness = timeliness,
                Limitations = limitations
            });
        }

        private double CalculateAccuracy(ActionableInsight[] insights)
        {
            if (insights.Length == 0) return 0;

            // Simplified accuracy based on confidence scores
            return insights.Average(i => i.ActionabilityScore);
        }

        private double CalculateRelevance(ActionableInsight[] insights)
        {
            if (insights.Length == 0) return 0;

            // Simplified relevance based on impact scores
            return insights.Average(i => i.Impact);
        }

        private double CalculateTimeliness(ActionableInsight[] insights, MetricValue[] metrics)
        {
            if (insights.Length == 0 || metrics == null || metrics.Length == 0) return 0.5;

            var latestMetricDate = metrics.Max(m => m.Timestamp);
            var insightDate = DateTime.UtcNow; // Assuming insights are current

            var daysDiff = (insightDate - latestMetricDate).TotalDays;

            return daysDiff switch
            {
                <= 1 => 1.0,
                <= 7 => 0.8,
                <= 30 => 0.6,
                _ => 0.3
            };
        }

        private string[] IdentifyLimitations(ActionableInsight[] insights)
        {
            var limitations = new List<string>();

            if (insights.Any(i => i.ActionabilityScore < 0.6))
                limitations.Add("Some insights have low actionability");

            if (insights.Any(i => i.Impact < 0.5))
                limitations.Add("Some insights have limited potential impact");

            if (insights.Length < 3)
                limitations.Add("Limited number of insights generated");

            if (insights.GroupBy(i => i.Type).Count() < 2)
                limitations.Add("Limited variety in insight types");

            return limitations.ToArray();
        }
    }
}