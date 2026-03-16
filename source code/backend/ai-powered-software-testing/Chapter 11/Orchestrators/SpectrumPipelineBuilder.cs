

// Orchestrators/SpectrumPipelineBuilder.cs
using Chapter_11.Exceptions;
using Chapter_11.Interfaces;
using Chapter_11.Models.Analysis;
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;
using Chapter_11.Services;


// Updated Orchestrators/SpectrumPipelineBuilder.cs

namespace Chapter_11.Orchestrators
{
    public class SpectrumPipelineBuilder : ISpectrumPipelineBuilder
    {
        private readonly ILLMService _llmService;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<SpectrumPipelineBuilder> _logger;

        public SpectrumPipelineBuilder(
            ILLMService llmService,
            IDatabaseService databaseService,
            ILogger<SpectrumPipelineBuilder> logger)
        {
            _llmService = llmService;
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<Pipeline> BuildPipelineAsync(
            StageAnalysis stageAnalysis,
            GateMapping gateMapping,
            SpectrumCoverage coverage,
            FeedbackMechanism[] feedbackMechanisms)
        {
            try
            {
                _logger.LogInformation("Building pipeline with {Coverage} coverage for {StageCount} stages",
                    coverage, stageAnalysis.Stages?.Length ?? 0);

                // Check for conflicts
                var conflicts = FindConflicts(stageAnalysis.Stages);
                if (conflicts.Any())
                {
                    throw new PipelineConflictException(
                        "Pipeline conflicts detected",
                        conflicts.ToArray(),
                        new[] { "Circular dependency detected between stages", "Review stage dependencies" }
                    );
                }

                // Use LLM to optimize pipeline configuration
                var prompt = $"Create optimal pipeline configuration for {stageAnalysis.Stages?.Length ?? 0} stages with {gateMapping.Gates?.Length ?? 0} quality gates and {coverage} coverage.";
                var pipelineConfig = await _llmService.GenerateCompletionAsync(prompt);

                var stages = stageAnalysis.Stages?.Select(s => new PipelineStage
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = s.Name,
                    Activities = s.Activities ?? Array.Empty<string>(),
                    Metrics = new[] { "Duration", "Success Rate", "Quality Score" }
                }).ToArray() ?? Array.Empty<PipelineStage>();

                var pipeline = new Pipeline
                {
                    Stages = stages,
                    QualityGates = gateMapping.Gates ?? Array.Empty<QualityGate>()
                };

                // Save pipeline configuration
                await _databaseService.SaveAsync(pipeline);

                return pipeline;
            }
            catch (PipelineConflictException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build pipeline");
                throw new PipelineConflictException(
                    "Unexpected error during pipeline build",
                    new[] { "Pipeline configuration error" },
                    new[] { ex.Message }
                );
            }
        }

        private List<string> FindConflicts(DevelopmentStage[] stages)
        {
            var conflicts = new List<string>();
            if (stages == null) return conflicts;

            // Check for circular dependencies
            var dependencyGraph = new Dictionary<string, HashSet<string>>();

            foreach (var stage in stages)
            {
                if (stage.Dependencies != null)
                {
                    dependencyGraph[stage.Id] = new HashSet<string>(stage.Dependencies);
                }
            }

            // Detect cycles
            foreach (var stage in stages)
            {
                if (HasCycle(stage.Id, dependencyGraph, new HashSet<string>()))
                {
                    conflicts.Add($"Circular dependency detected involving stage: {stage.Name ?? stage.Id}");
                }
            }

            return conflicts;
        }

        private bool HasCycle(string node, Dictionary<string, HashSet<string>> graph, HashSet<string> visited)
        {
            if (visited.Contains(node)) return true;
            if (!graph.ContainsKey(node)) return false;

            visited.Add(node);

            foreach (var dependency in graph[node])
            {
                if (HasCycle(dependency, graph, visited))
                    return true;
            }

            visited.Remove(node);
            return false;
        }
    }
}
