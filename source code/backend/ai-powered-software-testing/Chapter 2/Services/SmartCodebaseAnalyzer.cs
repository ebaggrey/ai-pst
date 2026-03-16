
    using Chapter_2.Exceptions;
    using Chapter_2.Models;
    using Chapter_2.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using System.Diagnostics;


namespace Chapter_2.Services
{
    public class SmartCodebaseAnalyzer : ICodebaseAnalyzer
    {
        private readonly ILogger<SmartCodebaseAnalyzer> _logger;
        private readonly ICodeAnalyzer _codeAnalyzer;
        private readonly ITestPatternRecognizer _patternRecognizer;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OnboardingSettings _settings;

        public SmartCodebaseAnalyzer(
            ILogger<SmartCodebaseAnalyzer> logger,
            ICodeAnalyzer codeAnalyzer,
            ITestPatternRecognizer patternRecognizer,
            IHttpClientFactory httpClientFactory,
            IOptions<OnboardingSettings> settings)
        {
            _logger = logger;
            _codeAnalyzer = codeAnalyzer;
            _patternRecognizer = patternRecognizer;
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
        }

        public async Task<CodeAnalysis> AnalyzeAsync(FirstImpressionRequest request, TimeSpan timeout)
        {
            _logger.LogInformation("Starting codebase analysis for {RepositoryUrl} with focus areas: {FocusAreas}",
                MaskSensitiveUrl(request.RepositoryUrl),
                string.Join(", ", request.FocusAreas));

            var stopwatch = Stopwatch.StartNew();
            var cancellationTokenSource = new CancellationTokenSource(timeout);

            try
            {
                // Step 1: Clone repository or access codebase
                var codebasePath = await CloneOrAccessRepositoryAsync(request, cancellationTokenSource.Token);

                // Step 2: Analyze code structure
                var structure = await _codeAnalyzer.AnalyzeStructureAsync(codebasePath);

                // Step 3: Analyze test patterns
                var testPatterns = await _patternRecognizer.RecognizePatternsAsync(codebasePath);

                // Step 4: Calculate metrics for each file
                var fileMetrics = await AnalyzeFileMetricsAsync(structure, cancellationTokenSource.Token);

                // Step 5: Identify technical debt
                var technicalDebt = await IdentifyTechnicalDebtAsync(structure, fileMetrics, request.FocusAreas);

                // Step 6: Calculate test coverage
                var testCoverage = await CalculateTestCoverageAsync(structure, codebasePath);

                // Step 7: Generate insights
                var insights = await GenerateInsightsAsync(structure, fileMetrics, testPatterns, technicalDebt, request);

                // Step 8: Calculate confidence score
                var confidenceScore = CalculateConfidenceScore(structure, fileMetrics, testCoverage);

                stopwatch.Stop();

                _logger.LogInformation("Codebase analysis completed in {ElapsedMilliseconds}ms with confidence {ConfidenceScore}",
                    stopwatch.ElapsedMilliseconds, confidenceScore);

                return new CodeAnalysis
                {
                    Id = Guid.NewGuid().ToString(),
                    AnalyzedAt = DateTime.UtcNow,
                    TechnicalDebt = technicalDebt,
                    TestCoverage = testCoverage,
                    ConfidenceScore = confidenceScore,
                    Insights = insights,
                    Metrics = new Dictionary<string, object>
                    {
                        ["analysisDurationMs"] = stopwatch.ElapsedMilliseconds,
                        ["totalFilesAnalyzed"] = structure.Files?.Length ?? 0,
                        ["totalProjects"] = structure.Projects?.Length ?? 0,
                        ["focusAreas"] = request.FocusAreas,
                        ["repositoryUrl"] = MaskSensitiveUrl(request.RepositoryUrl)
                    }
                };
            }
            catch (OperationCanceledException) when (cancellationTokenSource.Token.IsCancellationRequested)
            {
                _logger.LogWarning("Codebase analysis timed out after {Timeout} seconds", timeout.TotalSeconds);
                throw new AnalysisTimeoutException(timeout, $"Analysis exceeded timeout of {timeout.TotalSeconds} seconds");
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Failed to access repository {RepositoryUrl}", request.RepositoryUrl);
                throw new RepositoryAccessException(request.RepositoryUrl,
                    $"Failed to access repository: {httpEx.Message}",
                    "HTTP_ERROR", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during codebase analysis");
                throw;
            }
        }

        public async Task<CodeAnalysis> GeneratePartialAnalysisAsync(FirstImpressionRequest request)
        {
            _logger.LogInformation("Generating partial analysis for {RepositoryUrl}", request.RepositoryUrl);

            try
            {
                // For partial analysis, we analyze a subset of the codebase
                var structure = await AnalyzePartialStructureAsync(request);
                var testCoverage = new TestCoverageBreakdown
                {
                    Overall = 0,
                    Unit = 0,
                    Integration = 0,
                    EndToEnd = 0,
                    LastUpdated = DateTime.UtcNow
                };

                var insights = new CodebaseInsights
                {
                    HighestRiskArea = "Analysis incomplete",
                    Strengths = Array.Empty<string>(),
                    Weaknesses = new[] { "Analysis could not complete fully" },
                    QuickWinOpportunities = Array.Empty<string>(),
                    Recommendations = new Dictionary<string, string>
                    {
                        ["nextStep"] = "Complete full analysis when possible"
                    }
                };

                return new CodeAnalysis
                {
                    Id = Guid.NewGuid().ToString(),
                    AnalyzedAt = DateTime.UtcNow,
                    TechnicalDebt = Array.Empty<TechnicalDebt>(),
                    TestCoverage = testCoverage,
                    ConfidenceScore = 0.5,
                    Insights = insights,
                    Metrics = new Dictionary<string, object>
                    {
                        ["status"] = "partial",
                        ["message"] = "Analysis was interrupted or timed out",
                        ["repositoryUrl"] = MaskSensitiveUrl(request.RepositoryUrl)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating partial analysis");
                return CreateMinimalAnalysis(request);
            }
        }

        #region Private Methods

        private async Task<string> CloneOrAccessRepositoryAsync(FirstImpressionRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Accessing repository: {RepositoryUrl}", MaskSensitiveUrl(request.RepositoryUrl));

            // For local paths, just return the path
            if (Uri.TryCreate(request.RepositoryUrl, UriKind.Absolute, out var uri))
            {
                if (uri.IsFile)
                {
                    return uri.LocalPath;
                }
            }

            // For remote repositories, clone to temp directory
            var tempPath = Path.Combine(Path.GetTempPath(), "onboarding-analysis", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {
                if (request.RepositoryUrl.Contains("github.com", StringComparison.OrdinalIgnoreCase))
                {
                    await CloneGitRepositoryAsync(request.RepositoryUrl, tempPath, request.Branch, cancellationToken);
                }
                else if (request.RepositoryUrl.Contains("dev.azure.com", StringComparison.OrdinalIgnoreCase) ||
                         request.RepositoryUrl.Contains("visualstudio.com", StringComparison.OrdinalIgnoreCase))
                {
                    await CloneAzureDevOpsRepositoryAsync(request.RepositoryUrl, tempPath, cancellationToken);
                }
                else
                {
                    throw new RepositoryAccessException(request.RepositoryUrl,
                        $"Unsupported repository provider: {request.RepositoryUrl}",
                        "UNSUPPORTED_PROVIDER");
                }

                return tempPath;
            }
            catch
            {
                // Clean up temp directory on failure
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
                throw;
            }
        }

        private async Task CloneGitRepositoryAsync(string repositoryUrl, string targetPath, string branch, CancellationToken cancellationToken)
        {
            // In a real implementation, this would use libgit2sharp or git command line
            // For now, simulate with a delay
            await Task.Delay(1000, cancellationToken);

            _logger.LogDebug("Cloned Git repository to {TargetPath}", targetPath);

            // Simulate creating some test files for analysis
            SimulateCodebaseStructure(targetPath);
        }

        private async Task CloneAzureDevOpsRepositoryAsync(string repositoryUrl, string targetPath, CancellationToken cancellationToken)
        {
            // Similar to Git cloning but with Azure DevOps specifics
            await Task.Delay(1500, cancellationToken);
            _logger.LogDebug("Cloned Azure DevOps repository to {TargetPath}", targetPath);
            SimulateCodebaseStructure(targetPath);
        }

        private async Task<CodeStructure> AnalyzePartialStructureAsync(FirstImpressionRequest request)
        {
            // Analyze only a subset of files for partial analysis
            var tempPath = Path.GetTempPath();
            var structure = new Models.CodeStructure
            {
                RootPath = tempPath,
                Files = new[]
                {
                    new CodeFileInfo { Name = "Sample.cs", Path = Path.Combine(tempPath, "Sample.cs"), Size = 1024 }
                },
                Projects = Array.Empty<ProjectInfo>()
            };

            return structure;
        }

        private async Task<CodeMetrics[]> AnalyzeFileMetricsAsync(Models.CodeStructure? structure, CancellationToken cancellationToken)
        {
            var metrics = new List<CodeMetrics>();

            if (structure.Files != null)
            {
                foreach (var file in structure.Files.Take(10)) // Limit for demo
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var metric = await _codeAnalyzer.AnalyzeFileAsync(file.Path);
                    metrics.Add(metric);
                }
            }

            return metrics.ToArray();
        }

        private async Task<TechnicalDebt[]> IdentifyTechnicalDebtAsync(
            CodeStructure structure,
            CodeMetrics[] metrics,
            string[] focusAreas)
        {
            var technicalDebt = new List<TechnicalDebt>();

            // Analyze for common technical debt patterns
            if (focusAreas.Contains("code-quality", StringComparer.OrdinalIgnoreCase))
            {
                technicalDebt.Add(new TechnicalDebt
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "Missing null checks in public APIs",
                    Category = "Code Quality",
                    EffortDays = 2,
                    RiskLevel = "medium",
                    Recommendation = "Add parameter validation using Guard clauses"
                });

                technicalDebt.Add(new TechnicalDebt
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "High cyclomatic complexity in business logic methods",
                    Category = "Maintainability",
                    EffortDays = 3,
                    RiskLevel = "high",
                    Recommendation = "Refactor complex methods into smaller, testable units"
                });
            }

            if (focusAreas.Contains("testing", StringComparer.OrdinalIgnoreCase))
            {
                technicalDebt.Add(new TechnicalDebt
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "Missing unit tests for critical business logic",
                    Category = "Test Coverage",
                    EffortDays = 5,
                    RiskLevel = "high",
                    Recommendation = "Add comprehensive unit tests for core domain models"
                });

                technicalDebt.Add(new TechnicalDebt
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "Flaky integration tests due to timing issues",
                    Category = "Test Reliability",
                    EffortDays = 3,
                    RiskLevel = "medium",
                    Recommendation = "Implement proper waiting strategies and test data management"
                });
            }

            if (focusAreas.Contains("security", StringComparer.OrdinalIgnoreCase))
            {
                technicalDebt.Add(new TechnicalDebt
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = "Hardcoded credentials in configuration files",
                    Category = "Security",
                    EffortDays = 1,
                    RiskLevel = "critical",
                    Recommendation = "Move sensitive data to secure secret management"
                });
            }

            return technicalDebt.ToArray();
        }

        private async Task<TestCoverageBreakdown> CalculateTestCoverageAsync(Models.CodeStructure? structure, string codebasePath)
        {
            // In a real implementation, this would use tools like coverlet, dotCover, or read coverage reports
            // For now, simulate with reasonable defaults

            var random = new Random();

            return new TestCoverageBreakdown
            {
                Overall = 0.75m,
                Unit = 0.80m + (decimal)(random.NextDouble() * 0.1), // 80-90%
                Integration = 0.65m + (decimal)(random.NextDouble() * 0.1), // 65-75%
                EndToEnd = 0.60m + (decimal)(random.NextDouble() * 0.1), // 60-70%
                LastUpdated = DateTime.UtcNow,
                ByModule = new Dictionary<string, decimal>
                {
                    ["Domain"] = 0.85m,
                    ["Application"] = 0.75m,
                    ["Infrastructure"] = 0.70m,
                    ["Presentation"] = 0.65m
                },
                ByFileType = new Dictionary<string, decimal>
                {
                    [".cs"] = 0.78m,
                    [".js"] = 0.45m,
                    [".ts"] = 0.68m,
                    [".py"] = 0.55m
                }
            };
        }

        private async Task<CodebaseInsights> GenerateInsightsAsync(
            CodeStructure structure,
            CodeMetrics[] metrics,
            TestPattern[] testPatterns,
            TechnicalDebt[] technicalDebt,
            FirstImpressionRequest request)
        {
            var strengths = new List<string>
            {
                "Well-structured solution with clear separation of concerns",
                "Consistent naming conventions throughout the codebase",
                "Good use of dependency injection patterns"
            };

            var weaknesses = new List<string>
            {
                "Inconsistent error handling patterns",
                "Missing integration tests for critical workflows",
                "Some business logic classes have high coupling"
            };

            var quickWins = new List<string>
            {
                "Add null checks to public API methods",
                "Fix obvious compiler warnings",
                "Add missing XML documentation to public members"
            };

            var highestRisk = technicalDebt
                .OrderByDescending(td => GetRiskScore(td.RiskLevel))
                .ThenByDescending(td => td.EffortDays)
                .FirstOrDefault()?.Description ?? "Unknown";

            return new CodebaseInsights
            {
                HighestRiskArea = highestRisk,
                Strengths = strengths.ToArray(),
                Weaknesses = weaknesses.ToArray(),
                QuickWinOpportunities = quickWins.ToArray(),
                Recommendations = new Dictionary<string, string>
                {
                    ["immediate"] = "Address critical security vulnerabilities first",
                    ["shortTerm"] = "Improve test coverage for core business logic",
                    ["mediumTerm"] = "Refactor highly coupled components",
                    ["longTerm"] = "Implement comprehensive CI/CD pipeline with quality gates"
                }
            };
        }

        private double CalculateConfidenceScore(CodeStructure structure, CodeMetrics[] metrics, TestCoverageBreakdown testCoverage)
        {
            // Calculate confidence based on analysis completeness and data quality
            var fileCount = structure.Files?.Length ?? 0;
            var metricsCount = metrics.Length;

            if (fileCount == 0) return 0.1;

            var coverageScore = (double)testCoverage.Overall * 0.4;
            var metricsScore = Math.Min(1.0, (double)metricsCount / fileCount) * 0.3;
            var structureScore = (structure.Projects?.Length > 0 ? 0.2 : 0.1) + 0.1;

            return Math.Min(0.95, coverageScore + metricsScore + structureScore);
        }

        private int GetRiskScore(string riskLevel)
        {
            return riskLevel.ToLower() switch
            {
                "critical" => 4,
                "high" => 3,
                "medium" => 2,
                "low" => 1,
                _ => 0
            };
        }

        private string MaskSensitiveUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                return $"{uri.Scheme}://{uri.Host}/***";
            }
            catch
            {
                return "invalid-url/***";
            }
        }

        private CodeAnalysis CreateMinimalAnalysis(FirstImpressionRequest request)
        {
            return new CodeAnalysis
            {
                Id = Guid.NewGuid().ToString(),
                AnalyzedAt = DateTime.UtcNow,
                TechnicalDebt = Array.Empty<TechnicalDebt>(),
                TestCoverage = new TestCoverageBreakdown { LastUpdated = DateTime.UtcNow },
                ConfidenceScore = 0.1,
                Insights = new CodebaseInsights
                {
                    HighestRiskArea = "Unable to analyze",
                    Strengths = Array.Empty<string>(),
                    Weaknesses = new[] { "Failed to access or analyze codebase" }
                },
                Metrics = new Dictionary<string, object>
                {
                    ["status"] = "failed",
                    ["repositoryUrl"] = MaskSensitiveUrl(request.RepositoryUrl)
                }
            };
        }

        private void SimulateCodebaseStructure(string path)
        {
            // Create sample directory structure
            var directories = new[] { "src", "tests", "docs", "scripts" };
            foreach (var dir in directories)
            {
                Directory.CreateDirectory(Path.Combine(path, dir));
            }

            // Create sample files
            File.WriteAllText(Path.Combine(path, "src", "Program.cs"),
                "// Sample program\nConsole.WriteLine(\"Hello World\");");

            File.WriteAllText(Path.Combine(path, "tests", "UnitTest1.cs"),
                "[Test]\npublic void Test1() { Assert.Pass(); }");
        }

        #endregion
    }
}

