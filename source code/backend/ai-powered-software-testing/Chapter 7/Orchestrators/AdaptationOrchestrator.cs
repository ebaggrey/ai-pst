using Chapter_7.Interfaces;
using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;
using Chapter_7.Services.LLM;

namespace Chapter_7.Orchestrators
{
    public class AdaptationOrchestrator : IAdaptationOrchestrator
    {
        private readonly IPipelineAdapter _pipelineAdapter;
        private readonly ILLMService _llmService;
        private readonly ILogger<AdaptationOrchestrator> _logger;

        public AdaptationOrchestrator(
            IPipelineAdapter pipelineAdapter,
            ILLMService llmService,
            ILogger<AdaptationOrchestrator> logger)
        {
            _pipelineAdapter = pipelineAdapter;
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<AdaptationResponse> AdaptPipelineAsync(AdaptationRequest request)
        {
            // Get current pipeline using LLM
            var currentPipeline = await _llmService.GetCurrentPipelineAsync();

            // Determine adaptation needs using LLM
            var adaptationNeeds = await _llmService.DetermineAdaptationNeedsAsync(
                request.ChangeType,
                request.ImpactAssessment,
                currentPipeline);

            // Generate adaptation plan
            var adaptationPlan = await _pipelineAdapter.GenerateAdaptationPlanAsync(
                adaptationNeeds,
                currentPipeline,
                request.AdaptationStrategy,
                request.ValidationRules ?? Array.Empty<ValidationRule>());

            // Validate adaptation preserves functionality using LLM
            var validationResults = await _llmService.ValidateAdaptationPlanAsync(
                adaptationPlan,
                currentPipeline);

            // Generate rollback plan using LLM
            var rollbackPlan = await _llmService.GenerateRollbackPlanAsync(adaptationPlan, currentPipeline);

            // Estimate adaptation effort using LLM
            var effortEstimate = await _llmService.EstimateAdaptationEffortAsync(
                adaptationPlan,
                request.ImpactAssessment);

            // Generate implementation steps using LLM
            var implementationSteps = await _llmService.GenerateImplementationStepsAsync(adaptationPlan);

            // Generate testing strategy using LLM
            var testingStrategy = await _llmService.GenerateTestingStrategyAsync(adaptationPlan, currentPipeline);

            // Generate communication plan using LLM
            var communicationPlan = await _llmService.GenerateCommunicationPlanAsync(
                adaptationPlan,
                request.ImpactAssessment);

            return new AdaptationResponse
            {
                AdaptationId = Guid.NewGuid().ToString(),
                ChangeType = request.ChangeType,
                AdaptationPlan = adaptationPlan,
                ValidationResults = validationResults,
                RollbackPlan = rollbackPlan,
                EffortEstimate = effortEstimate,
                ImplementationSteps = implementationSteps,
                TestingStrategy = testingStrategy,
                CommunicationPlan = communicationPlan
            };
        }
    }
}
