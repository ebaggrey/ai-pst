using Chapter_1.Models;
using Chapter_1.Services.Interfaces;

namespace Chapter_1.Services
{
    public class FrontendPlugin : IArchitecturePlugin
    {
        private readonly ILogger<FrontendPlugin> _logger;

        public string PluginName => "Frontend Analyzer";

        public FrontendPlugin(ILogger<FrontendPlugin> logger)
        {
            _logger = logger;
        }

        public async Task<PluginAnalysisResult> AnalyzeAsync(ApplicationProfile profile)
        {
            return await Task.Run(() =>
            {
                _logger.LogDebug("Analyzing frontend architecture");

                var result = new PluginAnalysisResult
                {
                    PluginName = PluginName,
                    IntegrationPoints = new List<IntegrationPoint>(),
                    ExternalDependencies = 0,
                    ComplexityContribution = 0,
                    Findings = new List<string>(),
                    Recommendations = new List<string>()
                };

                if (profile.FrontendFrameworks != null)
                {
                    // Add integration points with backend
                    if (profile.BackendServices != null)
                    {
                        foreach (var service in profile.BackendServices)
                        {
                            result.IntegrationPoints.Add(new IntegrationPoint
                            {
                                Source = "Frontend",
                                Target = service.Name,
                                Protocol = "HTTP/REST",
                                FailureRate = 3
                            });
                        }
                    }

                    // Calculate complexity based on number of frameworks
                    result.ComplexityContribution = profile.FrontendFrameworks.Length * 0.8m;

                    // Add findings
                    if (profile.FrontendFrameworks.Length > 1)
                    {
                        result.Findings.Add("Multiple frontend frameworks increase testing complexity");
                        result.Recommendations.Add("Standardize on one framework for better testability");
                    }

                    if (profile.FrontendFrameworks.Contains("React"))
                    {
                        result.Recommendations.Add("Use React Testing Library for component tests");
                    }
                    else if (profile.FrontendFrameworks.Contains("Angular"))
                    {
                        result.Recommendations.Add("Use Angular TestBed for comprehensive testing");
                    }
                }

                return result;
            });
        }

        public bool CanAnalyze(ApplicationProfile profile)
        {
            return profile.FrontendFrameworks != null && profile.FrontendFrameworks.Any();
        }
    }
}