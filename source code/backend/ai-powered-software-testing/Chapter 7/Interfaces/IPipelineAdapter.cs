using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;

namespace Chapter_7.Interfaces
{
    public interface IPipelineAdapter
    {
        Task<AdaptationPlan> GenerateAdaptationPlanAsync(
            AdaptationNeeds adaptationNeeds,
            PipelineDefinition currentPipeline,
            AdaptationStrategy strategy,
            ValidationRule[] validationRules);
    }

    public class AdaptationNeeds
    {
        public string ChangeType { get; set; }
        public string[] RequiredModifications { get; set; }
    }
}
