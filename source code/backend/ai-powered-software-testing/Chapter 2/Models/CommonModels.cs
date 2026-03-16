namespace Chapter_2.Models
{
    public class CodeAnalysis
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
        public TechnicalDebt[] TechnicalDebt { get; set; } = Array.Empty<TechnicalDebt>();
        public TestCoverageBreakdown TestCoverage { get; set; } = new();
        public double ConfidenceScore { get; set; }
        public CodebaseInsights Insights { get; set; } = new();
        public Dictionary<string, object> Metrics { get; set; } = new();
    }

    public class TechnicalDebt
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal EffortDays { get; set; }
        public string RiskLevel { get; set; } = "medium"; // low, medium, high, critical
        public string[] FilePaths { get; set; } = Array.Empty<string>();
        public string Recommendation { get; set; } = string.Empty;
    }

    public class TestCoverageBreakdown
    {
        public decimal Overall { get; set; }
        public decimal Unit { get; set; }
        public decimal Integration { get; set; }
        public decimal EndToEnd { get; set; }
        public Dictionary<string, decimal> ByModule { get; set; } = new();
        public Dictionary<string, decimal> ByFileType { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class CodebaseInsights
    {
        public string HighestRiskArea { get; set; } = string.Empty;
        public string[] Strengths { get; set; } = Array.Empty<string>();
        public string[] Weaknesses { get; set; } = Array.Empty<string>();
        public string[] QuickWinOpportunities { get; set; } = Array.Empty<string>();
        public Dictionary<string, string> Recommendations { get; set; } = new();
    }

    public class RecommendationTimeline
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public string Phase { get; set; } = string.Empty; // immediate, short-term, medium-term, long-term
        public decimal EffortDays { get; set; }
        public string[] Dependencies { get; set; } = Array.Empty<string>();
        public string ExpectedImpact { get; set; } = string.Empty;
        public string[] RequiredSkills { get; set; } = Array.Empty<string>();
        public int Priority { get; set; } // 1-5, where 1 is highest
    }

    public class LearningPathRequest
    {
        public string TargetRole { get; set; } = string.Empty;
        public string[] CurrentSkills { get; set; } = Array.Empty<string>();
        public string[] DesiredSkills { get; set; } = Array.Empty<string>();
        public int TimelineDays { get; set; } = 90;
        public string LearningStyle { get; set; } = "balanced"; // theoretical, practical, balanced
        public int HoursPerWeek { get; set; } = 15;
    }

    public class LearningPath
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Phase[] Phases { get; set; } = Array.Empty<Phase>();
        public Milestone[] Milestones { get; set; } = Array.Empty<Milestone>();
        public Resource[] Resources { get; set; } = Array.Empty<Resource>();
        public SuccessCriteria SuccessCriteria { get; set; } = new();
    }

    public class Phase
    {
        public string Name { get; set; } = string.Empty;
        public int DurationDays { get; set; }
        public string[] Topics { get; set; } = Array.Empty<string>();
        public string[] Activities { get; set; } = Array.Empty<string>();
        public string[] Deliverables { get; set; } = Array.Empty<string>();
    }

    public class Milestone
    {
        public string Name { get; set; } = string.Empty;
        public int Day { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ValidationMethod { get; set; } = string.Empty;
    }

    public class Resource
    {
        public string Type { get; set; } = string.Empty; // video, article, course, lab
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string EstimatedTime { get; set; } = string.Empty;
    }

    public class SuccessCriteria
    {
        public string[] KnowledgeChecks { get; set; } = Array.Empty<string>();
        public string[] PracticalExercises { get; set; } = Array.Empty<string>();
        public string[] Assessments { get; set; } = Array.Empty<string>();
    }

    public class InterviewQuestionsResponse
    {
        public Question[] Questions { get; set; } = Array.Empty<Question>();
        public Dictionary<string, string[]> QuestionCategories { get; set; } = new();
        public string[] UsageTips { get; set; } = Array.Empty<string>();
        public string GeneratedAt { get; set; } = DateTime.UtcNow.ToString("o");
    }

    public class Question
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string[] FollowUps { get; set; } = Array.Empty<string>();
        public string EvaluationCriteria { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "medium";
    }

    public class FirstTestFix
    {
        public string OriginalTest { get; set; } = string.Empty;
        public string FixedTest { get; set; } = string.Empty;
        public Change[] Changes { get; set; } = Array.Empty<Change>();
        public string Explanation { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public FixMetrics Metrics { get; set; } = new();
    }

    public class Change
    {
        public string Type { get; set; } = string.Empty; // add, remove, modify, refactor
        public string Description { get; set; } = string.Empty;
        public string Before { get; set; } = string.Empty;
        public string After { get; set; } = string.Empty;
        public int LineNumber { get; set; }
    }

    public class FixMetrics
    {
        public int WaitStatementsBefore { get; set; }
        public int WaitStatementsAfter { get; set; }
        public int ComplexityBefore { get; set; }
        public int ComplexityAfter { get; set; }
        public decimal ReadabilityScoreChange { get; set; }
        public decimal StabilityImprovement { get; set; }
        public int LinesChanged { get; set; }
    }

    public class FirstFixResponse
    {
        public string OriginalTest { get; set; } = string.Empty;
        public string FixedTest { get; set; } = string.Empty;
        public Change[] ChangesMade { get; set; } = Array.Empty<Change>();
        public string Explanation { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public FixMetrics BeforeAfterMetrics { get; set; } = new();
        public string NextLearningStep { get; set; } = string.Empty;
    }

    public class InterviewInsights
    {
        public Question[] Questions { get; set; } = Array.Empty<Question>();
        public Dictionary<string, string[]> QuestionCategories { get; set; } = new();
        public string[] FollowUpPrompts { get; set; } = Array.Empty<string>();
        public TimingSuggestions TimingSuggestions { get; set; } = new();
    }

    public class TimingSuggestions
    {
        public int TotalMinutes { get; set; }
        public int WarmupMinutes { get; set; }
        public int TechnicalMinutes { get; set; }
        public int CulturalMinutes { get; set; }
        public int QAMinutes { get; set; }
    }
}
