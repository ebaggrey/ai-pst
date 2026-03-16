using Chapter_4.Models.Domain;

namespace Chapter_4.Helpers
{
    // Helpers/AITestingHelpers.cs (partial implementation)
    public static class AITestingHelpers
    {
        public static async Task<AICapabilityReport> AddStatisticalSignificanceAsync(AICapabilityReport report)
        {
            await Task.Delay(50); // Simulate async work
                                  // Add statistical significance calculations
            return report;
        }

        public static List<Recommendation> GenerateProviderSpecificRecommendations(AICapabilityReport report)
        {
            var recommendations = new List<Recommendation>();

            if (report.OverallScore < 70)
            {
                recommendations.Add(new Recommendation
                {
                    Area = "Overall Performance",
                    Suggestion = "Consider switching to a different model or provider",
                    Priority = "high",
                    Impact = "Significant improvement in accuracy and reliability"
                });
            }

            if (report.DimensionScores.Any(d => d.Value < 60))
            {
                var weakDimensions = report.DimensionScores.Where(d => d.Value < 60).Select(d => d.Key);
                recommendations.Add(new Recommendation
                {
                    Area = "Weak Dimensions",
                    Suggestion = $"Focus on improving: {string.Join(", ", weakDimensions)}",
                    Priority = "medium",
                    Impact = "Targeted improvement in specific capabilities"
                });
            }

            return recommendations;
        }

        // Similar helper methods for other endpoints would be implemented here...
        // These would typically be in a separate service or helper class
    }
}
