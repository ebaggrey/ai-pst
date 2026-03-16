
// Models/Analysis/TestSuiteAnalysis.cs
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;

namespace Chapter_11.Models.Analysis
{
    public class TestSuiteAnalysis
    {
        public string Id { get; set; }
        public Test[] Tests { get; set; }
        public TestCategory[] Categories { get; set; }
        public TestDependency[] Dependencies { get; set; }
        public TestCoverageAnalysis Coverage { get; set; }
    }

    public class TestCategory
    {
        public string Name { get; set; }
        public string[] TestIds { get; set; }
    }

    public class TestDependency
    {
        public string TestId { get; set; }
        public string DependsOnTestId { get; set; }
        public string DependencyType { get; set; }
    }

    public class TestCoverageAnalysis
    {
        public double OverallCoverage { get; set; }
        public ComponentCoverage[] ComponentCoverage { get; set; }
    }
}