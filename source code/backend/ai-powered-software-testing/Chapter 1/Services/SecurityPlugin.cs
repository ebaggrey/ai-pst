using Chapter_1.Models;
using Chapter_1.Services.Interfaces;

namespace Chapter_1.Services
{
    public class SecurityPlugin : IArchitecturePlugin
    {
        private readonly ILogger<SecurityPlugin> _logger;

        public string PluginName => "Security Analyzer";

        public SecurityPlugin(ILogger<SecurityPlugin> logger)
        {
            _logger = logger;
        }

        public async Task<PluginAnalysisResult> AnalyzeAsync(ApplicationProfile profile)
        {
            return await Task.Run(() =>
            {
                _logger.LogDebug("Analyzing security requirements");

                var result = new PluginAnalysisResult
                {
                    PluginName = PluginName,
                    IntegrationPoints = new List<IntegrationPoint>(),
                    ExternalDependencies = 0,
                    ComplexityContribution = 0,
                    Findings = new List<string>(),
                    Recommendations = new List<string>()
                };

                // Check compliance requirements
                if (profile.ComplianceRequirements != null && profile.ComplianceRequirements.Any())
                {
                    foreach (var requirement in profile.ComplianceRequirements)
                    {
                        result.Findings.Add($"Compliance requirement: {requirement}");

                        switch (requirement.ToUpper())
                        {
                            case "GDPR":
                                result.Recommendations.Add("Implement data anonymization testing");
                                result.Recommendations.Add("Test data deletion workflows");
                                break;
                            case "PCI-DSS":
                                result.Recommendations.Add("Add encryption validation tests");
                                result.Recommendations.Add("Test payment data handling");
                                break;
                            case "HIPAA":
                                result.Recommendations.Add("Test audit logging mechanisms");
                                result.Recommendations.Add("Validate PHI data protection");
                                break;
                        }
                    }

                    result.ComplexityContribution += profile.ComplianceRequirements.Length * 0.3m;
                }

                // Add security-specific recommendations
                result.Recommendations.Add("Implement SAST/DAST in CI/CD pipeline");
                result.Recommendations.Add("Add security tests for authentication flows");
                result.Recommendations.Add("Test for OWASP Top 10 vulnerabilities");

                return result;
            });
        }

        public bool CanAnalyze(ApplicationProfile profile)
        {
            return profile.ComplianceRequirements != null &&
                   profile.ComplianceRequirements.Any() ||
                   profile.SecurityRequirements != null &&
                   profile.SecurityRequirements.Any();
        }
    }
}