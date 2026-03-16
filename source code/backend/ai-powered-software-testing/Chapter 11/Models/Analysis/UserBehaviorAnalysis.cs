
// Models/Analysis/UserBehaviorAnalysis.cs
namespace Chapter_11.Models.Analysis
{
    public class UserBehaviorAnalysis
    {
        public string Id { get; set; }
        public UserBehaviorPattern[]? Patterns { get; set; }
        public UserJourney[]? Journeys { get; set; }
        public BehaviorInsight[]? Insights { get; set; }
    }

    public class UserBehaviorPattern
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public double Frequency { get; set; }
        public TimeSpan? AverageDuration { get; set; }
        public string[]? CommonSequences { get; set; }
    }

    public class UserJourney
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public JourneyStep[] Steps { get; set; }
        public double CompletionRate { get; set; }
    }

    public class JourneyStep
    {
        public int Order { get; set; }
        public string Action { get; set; }
        public TimeSpan TypicalDuration { get; set; }
    }

    public class BehaviorInsight
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public double Confidence { get; set; }
        public string[] Recommendations { get; set; }
    }
}