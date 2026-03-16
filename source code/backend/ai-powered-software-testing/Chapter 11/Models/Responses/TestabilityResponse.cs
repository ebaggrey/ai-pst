
// Models/Responses/TestabilityResponse.cs
using Chapter_11.Models.Requests;

namespace Chapter_11.Models.Responses
{
    public class TestabilityResponse
    {
        public string AnalysisId { get; set; }
        public Codebase Codebase { get; set; }
        public TestabilityAnalysis Analysis { get; set; }
        public TestabilityScore TestabilityScore { get; set; }
        public Improvement[] Improvements { get; set; }
        public RefactoringRecommendation[] RefactoringRecommendations { get; set; }
        public ImpactAssessment ImpactAssessment { get; set; }
        public ImplementationRoadmap ImplementationRoadmap { get; set; }
        public MonitoringPlan MonitoringPlan { get; set; }
    }

    public class TestabilityAnalysis
    {
        public string Id { get; set; }
        public CodeSmell[] CodeSmells { get; set; }
        public DependencyIssue[] DependencyIssues { get; set; }
        public ComplexityMetric[] ComplexityMetrics { get; set; }
    }

    public class CodeSmell
    {
        public string Type { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
    }

    public class DependencyIssue
    {
        public string Dependency { get; set; }
        public string Issue { get; set; }
        public string Recommendation { get; set; }
    }

    public class ComplexityMetric
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public double Threshold { get; set; }
        public bool ExceedsThreshold { get; set; }
    }

    public class TestabilityScore
    {
        public int Score { get; set; }
        public ComponentScore[] ComponentScores { get; set; }
    }

    public class ComponentScore
    {
        public string ComponentName { get; set; }
        public int Score { get; set; }
    }

    public class Improvement
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int EstimatedEffort { get; set; }
        public int Impact { get; set; }
    }

    public class RefactoringRecommendation
    {
        public string Id { get; set; }
        public string Location { get; set; }
        public string CurrentPattern { get; set; }
        public string RecommendedPattern { get; set; }
        public string[] Steps { get; set; }
    }

    public class ImpactAssessment
    {
        public string Assessment { get; set; }
        public ImpactArea[] Areas { get; set; }
    }

    public class ImpactArea
    {
        public string Name { get; set; }
        public string Impact { get; set; }
        public double Confidence { get; set; }
    }

    public class ImplementationRoadmap
    {
        public RoadmapPhase[] Phases { get; set; }
    }

    public class RoadmapPhase
    {
        public string Name { get; set; }
        public string[] Tasks { get; set; }
        public string Duration { get; set; }
    }

    public class MonitoringPlan
    {
        public MonitoringMetric[] Metrics { get; set; }
    }

    public class MonitoringMetric
    {
        public string Name { get; set; }
        public string CollectionMethod { get; set; }
        public double Target { get; set; }
    }
}