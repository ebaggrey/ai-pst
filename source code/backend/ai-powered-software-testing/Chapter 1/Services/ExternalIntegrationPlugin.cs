using Chapter_1.Models;
using Chapter_1.Services.Interfaces;

namespace Chapter_1.Services
{
    public class ExternalIntegrationPlugin : IArchitecturePlugin
    {
        private readonly ILogger<ExternalIntegrationPlugin> _logger;

        public string PluginName => "External Integration Analyzer";

        public ExternalIntegrationPlugin(ILogger<ExternalIntegrationPlugin> logger)
        {
            _logger = logger;
        }

        public async Task<PluginAnalysisResult> AnalyzeAsync(ApplicationProfile profile)
        {
            return await Task.Run(() =>
            {
                _logger.LogDebug("Analyzing external integrations");

                var result = new PluginAnalysisResult
                {
                    PluginName = PluginName,
                    IntegrationPoints = new List<IntegrationPoint>(),
                    ExternalDependencies = 0,
                    ComplexityContribution = 0,
                    Findings = new List<string>(),
                    Recommendations = new List<string>()
                };

                if (profile.DataSources != null && profile.DataSources.Contains("external-apis"))
                {
                    // Add external API integration points
                    result.IntegrationPoints.Add(new IntegrationPoint
                    {
                        Source = "Application",
                        Target = "External APIs",
                        Protocol = "HTTP/REST",
                        FailureRate = 5 // Higher failure rate for external services
                    });

                    result.ExternalDependencies = 1;
                    result.ComplexityContribution = 1.5m;

                    // Add findings
                    result.Findings.Add("External API dependencies introduce reliability risks");
                    result.Recommendations.Add("Implement circuit breaker pattern for external calls");
                    result.Recommendations.Add("Add retry logic with exponential backoff");
                    result.Recommendations.Add("Mock external APIs in integration tests");
                    result.Recommendations.Add("Monitor external API availability and latency");
                }

                return result;
            });
        }

        public bool CanAnalyze(ApplicationProfile profile)
        {
            return profile.DataSources != null && profile.DataSources.Contains("external-apis");
        }
    }
}