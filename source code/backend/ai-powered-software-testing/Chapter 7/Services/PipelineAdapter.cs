using Chapter_7.Exceptions;
using Chapter_7.Interfaces;
using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;

namespace Chapter_7.Services
{
    public class PipelineAdapter : IPipelineAdapter
    {
        private readonly ILogger<PipelineAdapter> _logger;

        public PipelineAdapter(ILogger<PipelineAdapter> logger)
        {
            _logger = logger;
        }

        public async Task<AdaptationPlan> GenerateAdaptationPlanAsync(
            AdaptationNeeds adaptationNeeds,
            PipelineDefinition currentPipeline,
            AdaptationStrategy strategy,
            ValidationRule[] validationRules)
        {
            _logger.LogInformation("Generating adaptation plan for {ChangeType}",
                adaptationNeeds.ChangeType);

            await Task.Delay(150);

            var steps = new List<AdaptationStep>();

            foreach (var modification in adaptationNeeds.RequiredModifications)
            {
                steps.Add(new AdaptationStep
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = $"Modify {modification} for {adaptationNeeds.ChangeType}"
                });
            }

            if (steps.Count > 5 && strategy == AdaptationStrategy.Aggressive)
            {
                throw new AdaptationComplexityException(
                    "Change too complex for aggressive adaptation",
                    steps.Count);
            }

            return new AdaptationPlan
            {
                Steps = steps.ToArray()
            };
        }
    }
}
