
// Services/Analysis/OptimizationRiskAssessorService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class OptimizationRiskAssessorService : IOptimizationRiskAssessor
    {
        private readonly ILogger<OptimizationRiskAssessorService> _logger;

        public OptimizationRiskAssessorService(ILogger<OptimizationRiskAssessorService> logger)
        {
            _logger = logger;
        }

        public async Task<OptimizationRiskAssessment> AssessOptimizationRisksAsync(
            OptimizationRecommendation optimization,
            MetricValueAnalysis valueAnalysis)
        {
            _logger.LogInformation("Assessing optimization risks");

            var risks = new List<string>();
            var mitigations = new List<string>();

            // Risk: Deprecating valuable metrics
            if (optimization.DeprecatedMetrics?.Any() == true)
            {
                var highValueDeprecated = optimization.DeprecatedMetrics
                    .Where(m =>
                        valueAnalysis.MetricValues?.FirstOrDefault(v => v.MetricId == m)?.Value > 70)
                    .ToArray();

                if (highValueDeprecated.Any())
                {
                    risks.Add($"Deprecating {highValueDeprecated.Length} potentially valuable metrics");
                    mitigations.Add("Review value assessment for deprecated metrics");
                }
            }

            // Risk: Consolidation complexity
            if (optimization.Consolidations?.Length > 3)
            {
                risks.Add("Multiple consolidations may increase complexity");
                mitigations.Add("Implement consolidations incrementally");
            }

            // Risk: Data loss during transition
            risks.Add("Potential data loss during implementation");
            mitigations.Add("Ensure complete data backup before implementation");

            // Risk: Stakeholder resistance
            if (optimization.DeprecatedMetrics?.Any() == true)
            {
                risks.Add("Stakeholder resistance to metric removal");
                mitigations.Add("Communicate benefits clearly and involve stakeholders early");
            }

            return await Task.FromResult(new OptimizationRiskAssessment
            {
                Risks = risks.ToArray(),
                Mitigations = mitigations.ToArray()
            });
        }
    }
}