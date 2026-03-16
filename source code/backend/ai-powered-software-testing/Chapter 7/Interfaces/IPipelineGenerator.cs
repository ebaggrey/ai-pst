
using Chapter_7.Models.Requests;
using Chapter_7.Models.Responses;
namespace Chapter_7.Interfaces
{
    
    public interface IPipelineGenerator
    {
        Task<PipelineDefinition> GeneratePipelineAsync(
            PipelineRequirements requirements,
            TeamPractices teamPractices,
            OptimizationFocus optimizationFocus);
    }

    public class PipelineRequirements
    {
        public CodebaseAnalysis CodebaseAnalysis { get; set; }
        public PipelineConstraints Constraints { get; set; }
    }
}
