
// Exceptions/ConstraintImpossibleException.cs
namespace Chapter_9.Exceptions
{
    public class ConstraintImpossibleException : Exception
    {
        public double MinimumAdditionalTime { get; set; }
        public int FeaturesToRemove { get; set; }
        public object ConstraintAnalysis { get; set; }
        public object SuggestedAdjustments { get; set; }

        public ConstraintImpossibleException(string message,
            double minAdditionalTime,
            int featuresToRemove,
            object constraintAnalysis,
            object suggestedAdjustments) : base(message)
        {
            MinimumAdditionalTime = minAdditionalTime;
            FeaturesToRemove = featuresToRemove;
            ConstraintAnalysis = constraintAnalysis;
            SuggestedAdjustments = suggestedAdjustments;
        }
    }

    // Exceptions/CoverageImpossibleException.cs
    public class CoverageImpossibleException : Exception
    {
        public int MinimumTestsRequired { get; set; }
        public double AchievableConfidence { get; set; }
        public object CoverageGapAnalysis { get; set; }
        public object ConstraintImpact { get; set; }

        public CoverageImpossibleException(string message,
            int minTestsRequired,
            double achievableConfidence,
            object coverageGapAnalysis,
            object constraintImpact) : base(message)
        {
            MinimumTestsRequired = minTestsRequired;
            AchievableConfidence = achievableConfidence;
            CoverageGapAnalysis = coverageGapAnalysis;
            ConstraintImpact = constraintImpact;
        }
    }

    // Exceptions/CostCalculationException.cs
    public class CostCalculationException : Exception
    {
        public object MissingData { get; set; }
        public object EstimationChallenges { get; set; }

        public CostCalculationException(string message,
            object missingData,
            object estimationChallenges) : base(message)
        {
            MissingData = missingData;
            EstimationChallenges = estimationChallenges;
        }
    }

    // Exceptions/PreservationViolationException.cs
    public class PreservationViolationException : Exception
    {
        public object ViolatedRules { get; set; }
        public object AffectedTests { get; set; }

        public PreservationViolationException(string message,
            object violatedRules,
            object affectedTests) : base(message)
        {
            ViolatedRules = violatedRules;
            AffectedTests = affectedTests;
        }
    }

    // Exceptions/ROICalculationException.cs
    public class ROICalculationException : Exception
    {
        public object MissingData { get; set; }
        public object Limitations { get; set; }

        public ROICalculationException(string message,
            object missingData,
            object limitations) : base(message)
        {
            MissingData = missingData;
            Limitations = limitations;
        }
    }
}
