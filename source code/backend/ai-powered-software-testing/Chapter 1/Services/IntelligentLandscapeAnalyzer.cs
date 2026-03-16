using Chapter_1.Exceptions;
using Chapter_1.Models;
using Chapter_1.Services.Interfaces;
using System.Collections.Generic;

namespace Chapter_1.Services
{
    public class IntelligentLandscapeAnalyzer : ILandscapeAnalyzer
    {
        private readonly IEnumerable<IArchitecturePlugin> _plugins;
        private readonly IConfiguration _config;
        private readonly ILogger<IntelligentLandscapeAnalyzer> _logger;

        public IntelligentLandscapeAnalyzer(
            IEnumerable<IArchitecturePlugin> plugins,
            IConfiguration config,
            ILogger<IntelligentLandscapeAnalyzer> logger)
        {
            _plugins = plugins;
            _config = config;
            _logger = logger;
        }

        public async Task<ArchitectureAnalysis> AnalyzeAsync(ApplicationProfile profile)
        {
            try
            {
                _logger.LogInformation("Analyzing {AppName} architecture", profile.Name);

                var analysis = new ArchitectureAnalysis
                {
                    ApplicationName = profile.Name,
                    AnalysisTime = DateTime.UtcNow,
                    IntegrationPoints = new List<IntegrationPoint>(),
                    Dependencies = new DependenciesAnalysis
                    {
                        CriticalDependencies = new List<Dependency>()
                    },
                    Complexity = new ComplexityAssessment(),
                    TechDebt = new TechDebtAssessment()
                };

                // Use different plugins for different architecture aspects
                foreach (var plugin in _plugins.Where(p => p.CanAnalyze(profile)))
                {
                    try
                    {
                        _logger.LogDebug("Running plugin: {PluginName}", plugin.PluginName);
                        var pluginResult = await plugin.AnalyzeAsync(profile);
                        analysis.IntegratePluginResults(pluginResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Plugin {PluginName} failed but continuing analysis", plugin.PluginName);
                    }
                }

                // Calculate risk scores
                analysis.RiskScore = CalculateOverallRisk(analysis);
                analysis.TestingPriority = DetermineTestingPriority(analysis);

                _logger.LogDebug("Analysis complete - Risk: {RiskScore}, Priority: {Priority}",
                    analysis.RiskScore, analysis.TestingPriority);

                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to analyze application architecture for {AppName}", profile.Name);
                throw new ArchitectureAnalysisException(
                    $"Failed to analyze architecture for {profile.Name}",
                    profile.Name,
                    profile.ArchitectureType,
                    ex);
            }
        }

        public async Task<TestabilityReport> AssessTestabilityAsync(ArchitectureAnalysis analysis)
        {
            return await Task.Run(() =>
            {
                var report = new TestabilityReport
                {
                    ApplicationName = analysis.ApplicationName,
                    GeneratedAt = DateTime.UtcNow,
                    Factors = new List<TestabilityFactor>(),
                    Recommendations = Array.Empty<string>()
                };

                // Calculate testability score based on various factors
                var factors = new List<TestabilityFactor>();

                // Integration points impact testability
                factors.Add(new TestabilityFactor
                {
                    Category = "Integration Complexity",
                    Score = Math.Max(0, 10 - analysis.IntegrationPoints.Count),
                    Explanation = $"{analysis.IntegrationPoints.Count} integration points identified",
                    Improvements = new[]
                    {
                        "Add contract testing for each integration",
                        "Implement service virtualization for external dependencies"
                    }
                });

                // Dependency impact
                factors.Add(new TestabilityFactor
                {
                    Category = "Dependency Management",
                    Score = Math.Max(0, 10 - (analysis.Dependencies.ExternalCount * 2)),
                    Explanation = $"{analysis.Dependencies.ExternalCount} external dependencies",
                    Improvements = new[]
                    {
                        "Use dependency injection for better testability",
                        "Create mock implementations for external services"
                    }
                });

                // Complexity impact
                factors.Add(new TestabilityFactor
                {
                    Category = "Code Complexity",
                    Score = 10 - (analysis.Complexity.Score / 2),
                    Explanation = $"Complexity score: {analysis.Complexity.Score}/10",
                    Improvements = new[]
                    {
                        "Break down complex components",
                        "Add more unit tests for complex logic"
                    }
                });

                report.Factors = factors;
                report.TestabilityScore = factors.Average(f => f.Score);

                // Generate recommendations based on low scores
                var recommendations = new List<string>();
                if (report.TestabilityScore < 5)
                {
                    recommendations.Add("High priority: Improve testability through refactoring");
                }

                recommendations.AddRange(factors
                    .Where(f => f.Score < 6)
                    .SelectMany(f => f.Improvements));

                report.Recommendations = recommendations.ToArray();

                return report;
            });
        }

        public async Task<ComplexityAssessment> CalculateComplexityAsync(ApplicationProfile profile)
        {
            return await Task.Run(() =>
            {
                var assessment = new ComplexityAssessment
                {
                    Score = 0,
                    ComplexityDrivers = new List<string>()
                };

                // Calculate based on profile attributes
                var score = 0m;

                // Service count contributes to complexity
                score += profile.BackendServices?.Length * 0.5m ?? 0;

                // Frontend frameworks
                if (profile.FrontendFrameworks?.Length > 1)
                {
                    score += 2;
                    assessment.ComplexityDrivers.Add("Multiple frontend frameworks");
                }

                // Data sources
                if (profile.DataSources?.Length > 2)
                {
                    score += 1.5m;
                    assessment.ComplexityDrivers.Add("Multiple data sources");
                }

                // Architecture type
                score += profile.ArchitectureType?.ToLower() switch
                {
                    "microservices" => 3,
                    "serverless" => 2,
                    "hybrid" => 2.5m,
                    _ => 1
                };

                assessment.Score = Math.Min(score, 10);
                assessment.RecommendedApproach = assessment.Score switch
                {
                    > 8 => "Comprehensive testing strategy with heavy automation",
                    > 5 => "Balanced testing approach with key automation",
                    _ => "Simple testing strategy focusing on critical paths"
                };

                return assessment;
            });
        }

        public async Task<RiskAssessment> AssessRisksAsync(ApplicationProfile profile)
        {
            return await Task.Run(() =>
            {
                var assessment = new RiskAssessment
                {
                    Criticality = 5,
                    ComplianceRequirements = profile.ComplianceRequirements ?? Array.Empty<string>(),
                    DataSensitivity = new[] { "PII" },
                    RiskFactors = new List<RiskFactor>().ToArray()
                };

                // Add risk factors based on profile
                var factors = new List<RiskFactor>();

                if (profile.BackendServices?.Length > 5)
                {
                    factors.Add(new RiskFactor
                    {
                        Area = "Service Coordination",
                        Likelihood = 7,
                        Impact = 8,
                        Description = "Multiple services increase coordination complexity"
                    });
                }

                if (profile.DataSources?.Contains("external-apis") == true)
                {
                    factors.Add(new RiskFactor
                    {
                        Area = "External Dependencies",
                        Likelihood = 6,
                        Impact = 9,
                        Description = "External API availability and reliability"
                    });
                }

                if (profile.ExpectedUsers == UserScale.Enterprise)
                {
                    factors.Add(new RiskFactor
                    {
                        Area = "Scalability",
                        Likelihood = 5,
                        Impact = 9,
                        Description = "Enterprise scale requires robust performance"
                    });
                }

                assessment.RiskFactors = factors.ToArray();

                // Calculate overall criticality
                if (factors.Any())
                {
                    assessment.Criticality = (int)Math.Min(10, factors.Average(f => (f.Likelihood + f.Impact) / 2));
                }

                return assessment;
            });
        }

        private decimal CalculateOverallRisk(ArchitectureAnalysis analysis)
        {
            var factors = new[]
            {
                analysis.IntegrationPoints.Count * 0.3m,
                analysis.Dependencies.ExternalCount * 0.25m,
                analysis.Complexity.Score * 0.2m,
                analysis.TechDebt.Severity * 0.25m
            };

            return Math.Min(factors.Sum(), 10.0m); // Scale to 0-10
        }

        private string DetermineTestingPriority(ArchitectureAnalysis analysis)
        {
            var riskScore = analysis.RiskScore;

            return riskScore switch
            {
                >= 8 => "critical",
                >= 6 => "high",
                >= 4 => "medium",
                _ => "low"
            };
        }
    }
}