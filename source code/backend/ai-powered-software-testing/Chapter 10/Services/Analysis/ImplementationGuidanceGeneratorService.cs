
// Services/Analysis/ImplementationGuidanceGeneratorService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class ImplementationGuidanceGeneratorService : IImplementationGuidanceGenerator
    {
        private readonly ILogger<ImplementationGuidanceGeneratorService> _logger;

        public ImplementationGuidanceGeneratorService(ILogger<ImplementationGuidanceGeneratorService> logger)
        {
            _logger = logger;
        }

        public async Task<InsightImplementationGuidance> GenerateImplementationGuidanceAsync(
            ActionableInsight[] insights)
        {
            _logger.LogInformation("Generating implementation guidance for {InsightCount} insights",
                insights?.Length ?? 0);

            if (insights == null || insights.Length == 0)
            {
                return new InsightImplementationGuidance
                {
                    Prerequisites = Array.Empty<string>(),
                    Steps = Array.Empty<string>(),
                    Resources = Array.Empty<string>()
                };
            }

            var prerequisites = new List<string>();
            var steps = new List<string>();
            var resources = new List<string>();

            // Add common prerequisites
            prerequisites.Add("Team alignment on insights");
            prerequisites.Add("Data access and validation");
            prerequisites.Add("Stakeholder buy-in");

            // Generate steps based on insight types
            foreach (var insight in insights)
            {
                steps.AddRange(insight.RecommendedActions ?? Array.Empty<string>());
            }

            // Add unique steps
            steps = steps.Distinct().ToList();

            // Add resources needed
            resources.Add("Analytics tools");
            resources.Add("Implementation team");
            resources.Add("Monitoring dashboard");
            resources.Add("Documentation template");

            return await Task.FromResult(new InsightImplementationGuidance
            {
                Prerequisites = prerequisites.ToArray(),
                Steps = steps.ToArray(),
                Resources = resources.ToArray()
            });
        }
    }
}