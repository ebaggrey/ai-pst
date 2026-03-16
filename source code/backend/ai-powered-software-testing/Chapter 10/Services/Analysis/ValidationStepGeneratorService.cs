
// Services/Analysis/ValidationStepGeneratorService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class ValidationStepGeneratorService : IValidationStepGenerator
    {
        private readonly ILogger<ValidationStepGeneratorService> _logger;

        public ValidationStepGeneratorService(ILogger<ValidationStepGeneratorService> logger)
        {
            _logger = logger;
        }

        public async Task<ValidationStep[]> GenerateValidationStepsAsync(ActionableInsight[] insights)
        {
            _logger.LogInformation("Generating validation steps for {InsightCount} insights",
                insights?.Length ?? 0);

            var steps = new List<ValidationStep>();

            // Add common validation steps
            steps.Add(new ValidationStep
            {
                Step = "Data Quality Check",
                Method = "Verify data sources and completeness",
                ExpectedOutcome = "Data is complete and accurate"
            });

            steps.Add(new ValidationStep
            {
                Step = "Statistical Validation",
                Method = "Run statistical tests on underlying metrics",
                ExpectedOutcome = "Patterns are statistically significant"
            });

            if (insights != null)
            {
                foreach (var insight in insights)
                {
                    steps.Add(new ValidationStep
                    {
                        Step = $"Validate {insight.Title}",
                        Method = $"Cross-reference with historical data and domain expertise",
                        ExpectedOutcome = $"Insight accuracy confirmed with {insight.ActionabilityScore:P1} confidence"
                    });
                }
            }

            steps.Add(new ValidationStep
            {
                Step = "Peer Review",
                Method = "Present insights to domain experts",
                ExpectedOutcome = "Insights validated by subject matter experts"
            });

            steps.Add(new ValidationStep
            {
                Step = "Small-scale Implementation",
                Method = "Test insights on a subset of data/teams",
                ExpectedOutcome = "Measurable improvement observed"
            });

            return await Task.FromResult(steps.ToArray());
        }
    }
}