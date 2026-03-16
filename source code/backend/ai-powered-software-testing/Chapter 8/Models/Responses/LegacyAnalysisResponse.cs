namespace Chapter_8.Models.Responses
{
    // Models/Responses/LegacyAnalysisResponse.cs
   
        public class LegacyAnalysisResponse
        {
            public string AnalysisId { get; set; }
            public CodebaseSummary CodebaseSummary { get; set; }
            public BusinessLogicMap[] BusinessLogicMap { get; set; }
            public RiskHotspot[] RiskHotspots { get; set; }
            public HiddenAssumption[] HiddenAssumptions { get; set; }
            public ModernizationReadiness ModernizationReadiness { get; set; }
            public RecommendedAction[] RecommendedActions { get; set; }
            public ConfidenceScore[] ConfidenceScores { get; set; }
            public NextStep[] NextSteps { get; set; }
        }

        public class CodebaseSummary
        {
            public string Name { get; set; }
            public long TotalLines { get; set; }
            public int ComplexityScore { get; set; }
            public string[] PrimaryTechnologies { get; set; }
            public int DependencyCount { get; set; }
            public double TechnicalDebtEstimate { get; set; }
        }

        public class BusinessLogicMap
        {
            public string BusinessFlowId { get; set; }
            public string BusinessFlowDescription { get; set; }
            public string[] CodeLocations { get; set; }
            public double MappingConfidence { get; set; }
            public string[] Gaps { get; set; }
        }

        public class RiskHotspot
        {
            public string Id { get; set; }
            public string Location { get; set; }
            public string RiskType { get; set; }
            public int Severity { get; set; } // 1-10
            public string Description { get; set; }
            public string[] MitigationStrategies { get; set; }
        }

        public class HiddenAssumption
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public string Impact { get; set; }
            public bool IsValidated { get; set; }
        }

        public class ModernizationReadiness
        {
            public double ReadinessScore { get; set; } // 0-1
            public string[] Strengths { get; set; }
            public string[] Weaknesses { get; set; }
            public string[] Opportunities { get; set; }
            public string[] Threats { get; set; }
        }

        public class RecommendedAction
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int Priority { get; set; }
            public string EstimatedEffort { get; set; }
            public string[] Dependencies { get; set; }
        }

        public class ConfidenceScore
        {
            public string Metric { get; set; }
            public double Score { get; set; } // 0-1
            public string Explanation { get; set; }
        }

        public class NextStep
        {
            public string Step { get; set; }
            public string Owner { get; set; }
            public string Timeline { get; set; }
        }
    
}
