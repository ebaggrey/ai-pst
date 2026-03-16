
using Chapter2_Ext.Models;

namespace Chapter2_Ext  
{
    public interface IPatternService
    {
        Task<TestingPatternDto> EstablishPatternAsync(PatternEstablishmentRequest request);
        Task<TrainingMaterials> GenerateTrainingMaterialsAsync(TrainingGenerationRequest request);
        Task<PipelineBlueprint> CreateAutomationPipelineAsync(PipelineRequest request);
        Task<List<TestingPatternDto>> GetPatternsByAreaAsync(string area);
        Task<TestingPatternDto> GetPatternByIdAsync(string id);
    }

    public interface IPatternValidator
    {
        Task<bool> ValidatePattern(TestingPatternDto pattern);
        Task<List<string>> GetValidationIssues(TestingPatternDto pattern);
    }

    public interface IPatternGenerator
    {
        Task<TestingPatternDto> GeneratePatternFromExamples(string area, List<TestExampleDto> examples);
        Task<TestingPatternDto> EnhancePatternWithAI(TestingPatternDto pattern);
    }

    public interface ITrainingGenerator
    {
        Task<TrainingMaterials> CreateWorkshopMaterials(TestingPatternDto pattern, TrainingGenerationRequest config);
        Task<TrainingMaterials> CreateQuickStartGuide(TestingPatternDto pattern, string audience);
    }

    public interface IPipelineGenerator
    {
        Task<PipelineBlueprint> DesignPipeline(TestingPatternDto pattern, PipelineRequest config);
        Task<string> GeneratePipelineCode(PipelineBlueprint blueprint);
    }
}