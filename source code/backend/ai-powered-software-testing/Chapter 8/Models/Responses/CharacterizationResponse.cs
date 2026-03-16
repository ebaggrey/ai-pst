

using Chapter_8.Models.Requests;

namespace Chapter_8.Models.Responses
{
    // Models/Responses/CharacterizationResponse.cs
    
        public class CharacterizationResponse
        {
            public string TestSuiteId { get; set; }
            public LegacyBehavior LegacyBehavior { get; set; }
            public CharacterizationTest[] CharacterizationTests { get; set; }
            public TestDocumentation Documentation { get; set; }
            public TestHarness TestHarness { get; set; }
            public ValidationSuite ValidationSuite { get; set; }
            public CoverageReport CoverageReport { get; set; }
            public ConfidenceMetric[] ConfidenceMetrics { get; set; }
            public MaintenanceGuide MaintenanceGuide { get; set; }
        }

        public class CharacterizationTest
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string TestCode { get; set; }
            public string[] Inputs { get; set; }
            public string ExpectedOutput { get; set; }
            public string Category { get; set; }
        }

        public class TestDocumentation
        {
            public string Overview { get; set; }
            public TestScenario[] Scenarios { get; set; }
            public string[] Assumptions { get; set; }
            public string[] Limitations { get; set; }
        }

        public class TestScenario
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string[] CoveredBehaviors { get; set; }
        }

        public class TestHarness
        {
            public string SetupCode { get; set; }
            public string TeardownCode { get; set; }
            public string[] RequiredMocks { get; set; }
            public Dictionary<string, string> Configuration { get; set; }
        }

        public class ValidationSuite
        {
            public string[] ValidationRules { get; set; }
            public string[] ValidationTests { get; set; }
            public double ValidationCoverage { get; set; }
        }

        public class CoverageReport
        {
            public double CoveragePercentage { get; set; } // 0-1
            public string[] CoveredBehaviors { get; set; }
            public string[] UncoveredBehaviors { get; set; }
            public string[] Recommendations { get; set; }
        }

        public class ConfidenceMetric
        {
            public string Metric { get; set; }
            public double Value { get; set; } // 0-1
            public string Justification { get; set; }
        }

        public class MaintenanceGuide
        {
            public string[] CommonIssues { get; set; }
            public string[] TroubleshootingSteps { get; set; }
            public string[] UpdateProcedures { get; set; }
        }
    
}
