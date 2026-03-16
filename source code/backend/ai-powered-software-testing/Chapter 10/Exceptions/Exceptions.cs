
using Chapter_10.Models.Errors;

namespace Chapter_10.Exceptions
{
    public class ObjectiveAmbiguityException : Exception
    {
        public string[] AmbiguousObjectives { get; }
        public string[] ClarificationQuestions { get; }

        public ObjectiveAmbiguityException(string message,
            string[] ambiguousObjectives,
            string[] clarificationQuestions) : base(message)
        {
            AmbiguousObjectives = ambiguousObjectives;
            ClarificationQuestions = clarificationQuestions;
        }
    }

    public class BaselineInconsistencyException : Exception
    {
        public InconsistencyDetails InconsistencyDetails { get; }
        public DataQualityIssue[] DataQualityIssues { get; }

        public BaselineInconsistencyException(string message,
            InconsistencyDetails inconsistencyDetails,
            DataQualityIssue[] dataQualityIssues) : base(message)
        {
            InconsistencyDetails = inconsistencyDetails;
            DataQualityIssues = dataQualityIssues;
        }
    }

    public class PatternDetectionException : Exception
    {
        public string[] DetectionChallenges { get; }
        public string[] DataRequirements { get; }

        public PatternDetectionException(string message,
            string[] detectionChallenges,
            string[] dataRequirements) : base(message)
        {
            DetectionChallenges = detectionChallenges;
            DataRequirements = dataRequirements;
        }
    }

    public class InsightGenerationException : Exception
    {
        public string[] GenerationChallenges { get; }
        public string[] MetricLimitations { get; }

        public InsightGenerationException(string message,
            string[] generationChallenges,
            string[] metricLimitations) : base(message)
        {
            GenerationChallenges = generationChallenges;
            MetricLimitations = metricLimitations;
        }
    }

    public class OptimizationConflictException : Exception
    {
        public string[] ConflictingGoals { get; }
        public TradeOffAnalysis TradeOffAnalysis { get; }

        public OptimizationConflictException(string message,
            string[] conflictingGoals,
            TradeOffAnalysis tradeOffAnalysis) : base(message)
        {
            ConflictingGoals = conflictingGoals;
            TradeOffAnalysis = tradeOffAnalysis;
        }
    }
}
