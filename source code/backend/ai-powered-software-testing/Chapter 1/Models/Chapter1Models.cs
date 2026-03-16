using Chapter_1.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Chapter_1.Models
{
    // Models/Landscape/ApplicationProfile.cs - Enhanced
    public enum UserScale
    {
        Small,
        Medium,
        Large,
        Enterprise
    }

    public class BackendService
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Technology { get; set; } = string.Empty;

        public string[] Endpoints { get; set; } = Array.Empty<string>();

        public bool HasDatabase { get; set; }

        public int RequestRatePerSecond { get; set; } = 100;

        // Add this property - it was missing
        public string[] Dependencies { get; set; } = Array.Empty<string>();

        public string HealthCheckEndpoint { get; set; } = "/health";

        // Additional properties that might be useful
        public string Version { get; set; } = "1.0.0";
        public bool IsCritical { get; set; } = false;
    }
   

    // Models/Landscape/RiskAssessment.cs
    public class RiskAssessment
    {
        [Range(1, 10)]
        public int Criticality { get; set; } = 5;

        public string[] ComplianceRequirements { get; set; } = Array.Empty<string>();

        public string[] DataSensitivity { get; set; } = Array.Empty<string>();

        public RiskFactor[] RiskFactors { get; set; } = Array.Empty<RiskFactor>();
    }

    public class RiskFactor
    {
        public string Area { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Likelihood { get; set; }

        [Range(1, 10)]
        public int Impact { get; set; }

        public string Description { get; set; } = string.Empty;
    }

    // Models/Landscape/ArchitectureAnalysis.cs
    public class ArchitectureAnalysis
    {
        public string ApplicationName { get; set; } = string.Empty;
        public DateTime AnalysisTime { get; set; }
        public decimal RiskScore { get; set; }
        public string TestingPriority { get; set; } = "medium";

        public List<IntegrationPoint> IntegrationPoints { get; set; } = new();
        public DependenciesAnalysis Dependencies { get; set; } = new();
        public ComplexityAssessment Complexity { get; set; } = new();
        public TechDebtAssessment TechDebt { get; set; } = new();

        public void IntegratePluginResults(PluginAnalysisResult result)
        {
            IntegrationPoints.AddRange(result.IntegrationPoints);
            Dependencies.ExternalCount += result.ExternalDependencies;
            Complexity.Score += result.ComplexityContribution;
            // Additional integration logic here
        }
    }

    public class IntegrationPoint
    {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Protocol { get; set; } = string.Empty;
        public int FailureRate { get; set; }
    }

    public class DependenciesAnalysis
    {
        public int ExternalCount { get; set; }
        public int InternalCount { get; set; }
        public List<Dependency> CriticalDependencies { get; set; } = new();
    }

    public class Dependency
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsThirdParty { get; set; }
        public string HealthStatus { get; set; } = "unknown";
    }

    public class ComplexityAssessment
    {
        [Range(0, 10)]
        public decimal Score { get; set; }
        public List<string> ComplexityDrivers { get; set; } = new();
        public string RecommendedApproach { get; set; } = string.Empty;
    }

    public class TechDebtAssessment
    {
        [Range(0, 10)]
        public decimal Severity { get; set; }
        public List<TechDebtItem> Items { get; set; } = new();
        public DateTime EstimatedResolution { get; set; }
    }

    public class TechDebtItem
    {
        public string Area { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public int EffortToFix { get; set; } // in days
    }

    // Models/Landscape/TestScenario.cs
    public class TestScenario
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "medium"; // high, medium, low
        public string[] RiskAreas { get; set; } = Array.Empty<string>();
        public string[] Steps { get; set; } = Array.Empty<string>();
        public string ExpectedOutcome { get; set; } = string.Empty;
        public string EstimatedDuration { get; set; } = "30 minutes";
    }

    // Models/Landscape/AutomationBlueprint.cs
    public class AutomationBlueprint
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string[] TechnologyStack { get; set; } = Array.Empty<string>();
        public string CodeSnippet { get; set; } = string.Empty;
        public string[] Coverage { get; set; } = Array.Empty<string>();
        public string[] Prerequisites { get; set; } = Array.Empty<string>();
        public string ImplementationComplexity { get; set; } = "medium";
    }

    // Models/Landscape/RiskHotspot.cs
    public class RiskHotspot
    {
        public string Area { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = "medium"; // critical, high, medium, low
        public string Description { get; set; } = string.Empty;
        public string[] Mitigation { get; set; } = Array.Empty<string>();
        public string TestApproach { get; set; } = string.Empty;
        public decimal Probability { get; set; }
        public decimal Impact { get; set; }
    }

    // Models/Landscape/FlakyTestPrediction.cs
    public class FlakyTestPrediction
    {
        public string TestType { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        [Range(0, 10)]
        public decimal FlakinessScore { get; set; }
        public string[] Reasons { get; set; } = Array.Empty<string>();
        public string[] StabilizationTips { get; set; } = Array.Empty<string>();
        public string[] WarningSigns { get; set; } = Array.Empty<string>();
    }

    // Models/Landscape/MonitoringStrategy.cs
    public class MonitoringStrategy
    {
        public List<MonitoringMetric> Metrics { get; set; } = new();
        public List<AlertRule> Alerts { get; set; } = new();
        public List<DashboardSuggestion> Dashboards { get; set; } = new();
        public string ImplementationGuide { get; set; } = string.Empty;
    }

    public class MonitoringMetric
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Threshold { get; set; }
        public string CollectionFrequency { get; set; } = "5 minutes";
        public string DataSource { get; set; } = string.Empty;
    }

    public class AlertRule
    {
        public string Condition { get; set; } = string.Empty;
        public string Severity { get; set; } = "warning"; // critical, warning, info
        public string Action { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
    }

    public class DashboardSuggestion
    {
        public string Name { get; set; } = string.Empty;
        public string[] Metrics { get; set; } = Array.Empty<string>();
        public string RefreshInterval { get; set; } = "1 minute";
        public string Purpose { get; set; } = string.Empty;
    }

    // Models/Landscape/TestabilityReport.cs
    public class TestabilityReport
    {
        public string ApplicationName { get; set; } = string.Empty;
        public decimal TestabilityScore { get; set; } // 0-10
        public List<TestabilityFactor> Factors { get; set; } = new();
        public string[] Recommendations { get; set; } = Array.Empty<string>();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class TestabilityFactor
    {
        public string Category { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string[] Improvements { get; set; } = Array.Empty<string>();
    }



}
