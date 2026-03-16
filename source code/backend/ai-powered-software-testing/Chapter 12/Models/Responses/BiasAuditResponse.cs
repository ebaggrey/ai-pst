

// Models/Responses/BiasAuditResponse.cs
namespace Chapter_12.Models.Responses
{
    public class BiasAuditResponse
    {
        public string DatasetName { get; set; }
        public string AuditId { get; set; }
        public DateTime AuditDate { get; set; }
        public List<BiasFinding> Findings { get; set; }
        public List<InclusiveSuggestion> Suggestions { get; set; }
        public BiasScore OverallBiasScore { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class BiasFinding
    {
        public string FieldName { get; set; }
        public string BiasType { get; set; }
        public string Description { get; set; }
        public double SeverityScore { get; set; }
        public List<string> Examples { get; set; }
    }

    public class InclusiveSuggestion
    {
        public string OriginalValue { get; set; }
        public string SuggestedValue { get; set; }
        public string FieldName { get; set; }
        public string Rationale { get; set; }
        public int ConfidenceScore { get; set; }
    }

    public class BiasScore
    {
        public double OverallScore { get; set; }
        public double GenderBiasScore { get; set; }
        public double RacialBiasScore { get; set; }
        public double AgeBiasScore { get; set; }
        public double CulturalBiasScore { get; set; }
        public string RiskLevel { get; set; }
    }
}