using Chapter_1.Models;
using Chapter_1.Services.Interfaces;

namespace Chapter_1.Services
{
    public class DatabasePlugin : IArchitecturePlugin
    {
        private readonly ILogger<DatabasePlugin> _logger;

        public string PluginName => "Database Analyzer";

        public DatabasePlugin(ILogger<DatabasePlugin> logger)
        {
            _logger = logger;
        }

        public async Task<PluginAnalysisResult> AnalyzeAsync(ApplicationProfile profile)
        {
            return await Task.Run(() =>
            {
                _logger.LogDebug("Analyzing database architecture");

                var result = new PluginAnalysisResult
                {
                    PluginName = PluginName,
                    IntegrationPoints = new List<IntegrationPoint>(),
                    ExternalDependencies = 0,
                    ComplexityContribution = 0,
                    Findings = new List<string>(),
                    Recommendations = new List<string>()
                };

                if (profile.DataSources != null && profile.DataSources.Contains("database"))
                {
                    // Add database integration points
                    if (profile.BackendServices != null)
                    {
                        foreach (var service in profile.BackendServices.Where(s => s.HasDatabase))
                        {
                            result.IntegrationPoints.Add(new IntegrationPoint
                            {
                                Source = service.Name,
                                Target = "Database",
                                Protocol = "SQL/TCP",
                                FailureRate = 1
                            });
                        }
                    }

                    // Calculate complexity contribution
                    result.ComplexityContribution = 2.0m;

                    // Add findings
                    result.Findings.Add("Database layer requires data consistency testing");
                    result.Recommendations.Add("Implement database migration testing");
                    result.Recommendations.Add("Use testcontainers for database integration tests");
                    result.Recommendations.Add("Add data integrity validation tests");
                }

                return result;
            });
        }

        public bool CanAnalyze(ApplicationProfile profile)
        {
            return profile.DataSources != null && profile.DataSources.Contains("database");
        }
    }
}