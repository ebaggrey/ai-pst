namespace Chapter_7.Models.Requests
{
  
    public class AdaptationRequest
    {
        public string ChangeType { get; set; }
        public ImpactAssessment ImpactAssessment { get; set; }
        public AdaptationStrategy AdaptationStrategy { get; set; }
        public ValidationRule[] ValidationRules { get; set; }
    }

    public class ImpactAssessment
    {
        public double ImpactScore { get; set; }
        public string[] AffectedComponents { get; set; }
    }

    public enum AdaptationStrategy
    {
        Conservative,
        Balanced,
        Aggressive
    }

    public class ValidationRule
    {
        public string RuleId { get; set; }
        public string Condition { get; set; }
    }
}
