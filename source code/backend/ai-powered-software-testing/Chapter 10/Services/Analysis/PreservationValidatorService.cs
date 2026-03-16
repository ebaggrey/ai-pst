
// Services/Analysis/PreservationValidatorService.cs
using Chapter_10.Analysis;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class PreservationValidatorService : IPreservationValidator
    {
        private readonly ILogger<PreservationValidatorService> _logger;

        public PreservationValidatorService(ILogger<PreservationValidatorService> logger)
        {
            _logger = logger;
        }

        public async Task<PreservationValidation> ValidatePreservationAsync(
            OptimizationRecommendation optimization,
            PreservationRule[] rules,
            MetricValueAnalysis valueAnalysis)
        {
            _logger.LogInformation("Validating preservation of {RuleCount} rules", rules?.Length ?? 0);

            if (rules == null || rules.Length == 0)
            {
                return new PreservationValidation
                {
                    IsPreserved = true,
                    ValidatedRules = Array.Empty<string>(),
                    Warnings = new[] { "No preservation rules defined" }
                };
            }

            var validatedRules = new List<string>();
            var warnings = new List<string>();

            foreach (var rule in rules)
            {
                bool isPreserved = CheckRulePreservation(rule, optimization, valueAnalysis);

                if (isPreserved)
                {
                    validatedRules.Add($"{rule.Rule} - preserved");
                }
                else
                {
                    warnings.Add($"Rule '{rule.Rule}' may be impacted by optimization");
                }
            }

            return await Task.FromResult(new PreservationValidation
            {
                IsPreserved = !warnings.Any(),
                ValidatedRules = validatedRules.ToArray(),
                Warnings = warnings.ToArray()
            });
        }

     
        private bool CheckRulePreservation(
            PreservationRule rule,
            OptimizationRecommendation optimization,
            MetricValueAnalysis valueAnalysis)
        {
            // Check if metric is being deprecated
            if (optimization.DeprecatedMetrics?.Contains(rule.MetricId) == true)
                return false;

            // Check if metric is being consolidated
            if (optimization.Consolidations?.Any(c => c.SourceMetricIds.Contains(rule.MetricId)) == true)
            {
                // For consolidations, check if the rule can be adapted
                return rule.Rule?.Contains("preserve") == true;
            }

            return true;
        }
    }
}