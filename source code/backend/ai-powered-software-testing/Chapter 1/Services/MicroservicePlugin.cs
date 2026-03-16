using Chapter_1.Models;
using Chapter_1.Services.Interfaces;

namespace Chapter_1.Services
{
    public class MicroservicePlugin : IArchitecturePlugin
    {
        private readonly ILogger<MicroservicePlugin> _logger;

        public string PluginName => "Microservice Analyzer";

        public MicroservicePlugin(ILogger<MicroservicePlugin> logger)
        {
            _logger = logger;
        }

        public async Task<PluginAnalysisResult> AnalyzeAsync(ApplicationProfile profile)
        {
            return await Task.Run(() =>
            {
                _logger.LogDebug("Analyzing microservices architecture");

                var result = new PluginAnalysisResult
                {
                    PluginName = PluginName,
                    IntegrationPoints = new List<IntegrationPoint>(),
                    ExternalDependencies = 0,
                    ComplexityContribution = 0,
                    Findings = new List<string>(),
                    Recommendations = new List<string>()
                };

                if (profile.BackendServices != null)
                {
                    foreach (var service in profile.BackendServices)
                    {
                        // Add integration points between services
                        if (service.Dependencies != null)
                        {
                            foreach (var dependency in service.Dependencies)
                            {
                                result.IntegrationPoints.Add(new IntegrationPoint
                                {
                                    Source = service.Name,
                                    Target = dependency,
                                    Protocol = "HTTP/REST",
                                    FailureRate = 2 // Default 2% failure rate
                                });
                            }
                        }

                        // Count external dependencies
                        result.ExternalDependencies += service.Dependencies?.Length ?? 0;
                    }

                    // Calculate complexity contribution
                    result.ComplexityContribution = profile.BackendServices.Length * 0.5m;

                    // Add findings
                    if (profile.BackendServices.Length > 10)
                    {
                        result.Findings.Add("High number of microservices may complicate testing");
                        result.Recommendations.Add("Consider service grouping for integration tests");
                    }
                }

                return result;
            });
        }

        public bool CanAnalyze(ApplicationProfile profile)
        {
            return profile.ArchitectureType?.ToLower() == "microservices";
        }
    }
}