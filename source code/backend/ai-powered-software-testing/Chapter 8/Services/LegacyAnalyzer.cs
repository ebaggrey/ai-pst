
using Chapter_8.Exceptions;
using Chapter_8.Interfaces;
using Chapter_8.Models.Requests;


namespace Chapter_8.Services
{
    public class LegacyAnalyzer : ILegacyAnalyzer
    {
        private readonly ILogger<LegacyAnalyzer> _logger;

        public LegacyAnalyzer(ILogger<LegacyAnalyzer> logger)
        {
            _logger = logger;
        }

        public async Task<CodebaseAnalysis> AnalyzeCodebaseAsync(LegacyAnalysisRequest request)
        {
            try
            {
                _logger.LogInformation("Starting analysis of codebase: {CodebaseName}",
                    request.Codebase.Name);

                // Simulate analysis work
                await Task.Delay(1000);

                // Check for complexity issues
                if (request.Codebase.ComplexityScore > 9)
                {
                    throw new CodeComplexityException(
                        "Codebase complexity exceeds analyzable threshold",
                        request.Codebase.ComplexityScore,
                        "Break down analysis into smaller modules")
                    {
                        ComplexityScore = request.Codebase.ComplexityScore,
                        SuggestedApproach = "Incremental analysis"
                    };
                }

                // Perform analysis
                var analysis = new CodebaseAnalysis
                {
                    AnalysisId = Guid.NewGuid().ToString(),
                    Structure = AnalyzeStructure(request.Codebase),
                    Complexity = CalculateComplexity(request.Codebase),
                    Dependencies = AnalyzeDependencies(request.Codebase),
                    CodeSmells = DetectCodeSmells(request.Codebase),
                    RawAnalysis = new Dictionary<string, object>
                    {
                        ["analysisDepth"] = request.AnalysisDepth,
                        ["timestamp"] = DateTime.UtcNow,
                        ["toolVersion"] = "2.1.0"
                    }
                };

                _logger.LogInformation("Analysis complete for {CodebaseName}",
                    request.Codebase.Name);

                return analysis;
            }
            catch (Exception ex) when (ex is not CodeComplexityException)
            {
                _logger.LogError(ex, "Error analyzing codebase {CodebaseName}",
                    request.Codebase.Name);
                throw;
            }
        }

        private CodeStructure AnalyzeStructure(CodebaseInfo codebase)
        {
            return new CodeStructure
            {
                Modules = new[]
                {
                    new Module
                    {
                        Name = "Core",
                        Path = "/src/core",
                        Classes = new[] { "BusinessLogic", "DataAccess" }
                    },
                    new Module
                    {
                        Name = "UI",
                        Path = "/src/ui",
                        Classes = new[] { "MainForm", "Controller" }
                    }
                },
                Classes = new[]
                {
                    new Class { Name = "BusinessLogic", Module = "Core", MethodCount = 25, ComplexityScore = 7 },
                    new Class { Name = "DataAccess", Module = "Core", MethodCount = 15, ComplexityScore = 5 }
                },
                Methods = new[]
                {
                    new Method { Name = "ProcessOrder", Class = "BusinessLogic", LinesOfCode = 150, CyclomaticComplexity = 12 },
                    new Method { Name = "SaveToDatabase", Class = "DataAccess", LinesOfCode = 45, CyclomaticComplexity = 4 }
                }
            };
        }

        private ComplexityMetrics CalculateComplexity(CodebaseInfo codebase)
        {
            return new ComplexityMetrics
            {
                AverageCyclomaticComplexity = 8.5,
                MaxCyclomaticComplexity = 25,
                CouplingScore = 0.65,
                CohesionScore = 0.72,
                Metrics = new Dictionary<string, double>
                {
                    ["maintainabilityIndex"] = 45,
                    ["technicalDebtRatio"] = 0.35,
                    ["commentDensity"] = 0.12
                }
            };
        }

        private DependencyGraph AnalyzeDependencies(CodebaseInfo codebase)
        {
            return new DependencyGraph
            {
                InternalDependencies = new[]
                {
                    new Dependency { Name = "BusinessLogic", Version = "1.0", IsExternal = false },
                    new Dependency { Name = "DataAccess", Version = "1.0", IsExternal = false }
                },
                ExternalDependencies = codebase.Dependencies ?? Array.Empty<Dependency>(),
                CircularDependencies = new[] { "BusinessLogic -> DataAccess -> BusinessLogic" }
            };
        }

        private CodeSmell[] DetectCodeSmells(CodebaseInfo codebase)
        {
            return new[]
            {
                new CodeSmell
                {
                    Type = "GodClass",
                    Location = "BusinessLogic",
                    Description = "Class has too many responsibilities",
                    Severity = 8
                },
                new CodeSmell
                {
                    Type = "LongMethod",
                    Location = "BusinessLogic.ProcessOrder",
                    Description = "Method has 150 lines of code",
                    Severity = 6
                },
                new CodeSmell
                {
                    Type = "ShotgunSurgery",
                    Location = "Multiple",
                    Description = "Changing validation requires changes in 5+ classes",
                    Severity = 7
                }
            };
        }
    }
}
