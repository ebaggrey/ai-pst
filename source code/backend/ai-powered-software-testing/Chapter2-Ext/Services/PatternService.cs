


using Chapter2_Ext.Models;
using System.Threading;

namespace Chapter2_Ext.Services
{
    public class PatternService : IPatternService
    {
        private readonly ILogger<PatternService> _logger;
        private readonly IPatternGenerator _patternGenerator;
        private readonly IPatternValidator _patternValidator;
        private readonly ITrainingGenerator _trainingGenerator;
        private readonly IPipelineGenerator _pipelineGenerator;
        private readonly List<TestingPatternDto> _patterns = new();

        public PatternService(
            ILogger<PatternService> logger,
            IPatternGenerator patternGenerator,
            IPatternValidator patternValidator,
            ITrainingGenerator trainingGenerator,
            IPipelineGenerator pipelineGenerator)
        {
            _logger = logger;
            _patternGenerator = patternGenerator;
            _patternValidator = patternValidator;
            _trainingGenerator = trainingGenerator;
            _pipelineGenerator = pipelineGenerator;
        }

        public async Task<TestingPatternDto> EstablishPatternAsync(PatternEstablishmentRequest request)
        {
            try
            {
                _logger.LogInformation("Establishing pattern for area: {Area}", request.Area);

                // Generate pattern from examples
                var pattern = await _patternGenerator.GeneratePatternFromExamples(
                    request.Area,
                    request.Examples);

                // Enhance with AI
                pattern = await _patternGenerator.EnhancePatternWithAI(pattern);

                // Validate the pattern
                var isValid = await _patternValidator.ValidatePattern(pattern);
                if (!isValid)
                {
                    var issues = await _patternValidator.GetValidationIssues(pattern);
                    throw new CustomExceptions.PatternValidationException(
                        $"Pattern validation failed: {string.Join(", ", issues)}",
                        new PatternError
                        {
                            PatternArea = request.Area,
                            FailureType = "validation",
                            Symptoms = issues,
                            RootCause = "Pattern does not meet validation criteria",
                            MitigationSteps = new List<string>
                            {
                                "Review the examples provided",
                                "Adjust validation criteria if needed",
                                "Provide more diverse examples"
                            }
                        });
                }

                // Set quality indicators based on request
                pattern.QualityIndicators = CalculateQualityIndicators(pattern, request);
                pattern.AdoptionMetrics = EstimateAdoptionMetrics(pattern, request);

                // Store the pattern
                _patterns.Add(pattern);

                _logger.LogInformation("Pattern established successfully. ID: {PatternId}", pattern.Id);
                return pattern;
            }
            catch (Exception ex) when (ex is not CustomExceptions.PatternEstablishmentException)
            {
                _logger.LogError(ex, "Error establishing pattern for area: {Area}", request.Area);
                throw new CustomExceptions.PatternGenerationException(
                    "Failed to establish pattern",
                    new PatternError
                    {
                        PatternArea = request.Area,
                        FailureType = "generation",
                        Symptoms = new List<string> { "Pattern generation process failed" },
                        RootCause = ex.Message,
                        MitigationSteps = new List<string>
                        {
                            "Check input data quality",
                            "Verify service dependencies",
                            "Retry with different examples"
                        }
                    });
            }
        }

        public async Task<TrainingMaterials> GenerateTrainingMaterialsAsync(TrainingGenerationRequest request)
        {
            try
            {
                _logger.LogInformation("Generating training materials for pattern: {PatternId}",
                    request.Pattern.Id);

                if (request.Format == "workshop-ready")
                {
                    return await _trainingGenerator.CreateWorkshopMaterials(
                        request.Pattern, request);
                }
                else
                {
                    return await _trainingGenerator.CreateQuickStartGuide(
                        request.Pattern, request.Audience);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating training materials");
                throw;
            }
        }

        public async Task<PipelineBlueprint> CreateAutomationPipelineAsync(PipelineRequest request)
        {
            try
            {
                _logger.LogInformation("Creating pipeline for pattern: {PatternId}",
                    request.PatternId);

                // Get the pattern
                var pattern = await GetPatternByIdAsync(request.PatternId);
                if (pattern == null)
                {
                    throw new ArgumentException($"Pattern not found: {request.PatternId}");
                }

                // Design the pipeline
                return await _pipelineGenerator.DesignPipeline(pattern, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating automation pipeline");
                throw;
            }
        }

        public  Task<List<TestingPatternDto>> GetPatternsByAreaAsync(string area)
        {
            return System.Threading.Tasks.Task.FromResult(_patterns
                .Where(p => p.Area.Equals(area, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }

        public Task<TestingPatternDto> GetPatternByIdAsync(string id)
        {
            return System.Threading.Tasks.Task.FromResult(_patterns.FirstOrDefault(p => p.Id == id));
        }

        private QualityIndicators CalculateQualityIndicators(
            TestingPatternDto pattern,
            PatternEstablishmentRequest request)
        {
            // Simplified calculation - in reality, this would use more sophisticated logic
            var repeatabilityScore = request.DesiredConsistency switch
            {
                "high" => 90,
                "medium" => 75,
                "low" => 60,
                _ => 70
            };

            var learningCurve = pattern.Implementation.CodeExamples.Count > 5 ? "easy" : "medium";
            var maintenanceCost = request.AutomationLevel == "fully-automated" ? "low" : "medium";

            return new QualityIndicators
            {
                RepeatabilityScore = repeatabilityScore,
                LearningCurve = learningCurve,
                MaintenanceCost = maintenanceCost
            };
        }

        private AdoptionMetrics EstimateAdoptionMetrics(
            TestingPatternDto pattern,
            PatternEstablishmentRequest request)
        {
            var timeSavePercentage = request.AutomationLevel switch
            {
                "manual" => "10-20%",
                "semi-automated" => "40-60%",
                "fully-automated" => "70-90%",
                _ => "30-50%"
            };

            var teamSizeFactor = request.Metadata?.TeamSize ?? 5;
            var satisfactionScore = Math.Min(10, 5 + (teamSizeFactor / 2));

            return new AdoptionMetrics
            {
                EstimatedTimeSave = timeSavePercentage,
                ErrorReduction = "50-70%",
                TeamSatisfaction = satisfactionScore
            };
        }
    }
}
