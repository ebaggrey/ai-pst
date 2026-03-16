// Models/Responses/ShiftRightResponse.cs
using Chapter_11.Models.Requests;

namespace Chapter_11.Models.Responses
{
    public class ShiftRightResponse
    {
        public string MonitorsId { get; set; }
        public ProductionSystem ProductionSystem { get; set; }
        public Monitor[] Monitors { get; set; }
        public FeedbackLoop[] FeedbackLoops { get; set; }
        public IncidentResponse IncidentResponse { get; set; }
        public MonitoringCoverage CoverageAssessment { get; set; }
        public AlertConfiguration AlertConfiguration { get; set; }
        public IntegrationPlan IntegrationPlan { get; set; }
        public CostBenefitAnalysis CostBenefitAnalysis { get; set; }
    }

    public class Monitor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Configuration { get; set; }
        public string[] Targets { get; set; }
    }

    public class IncidentResponse
    {
        public IncidentRule[] Rules { get; set; }
        public ActionPlan[] ActionPlans { get; set; }
    }

    public class IncidentRule
    {
        public string Condition { get; set; }
        public string Severity { get; set; }
        public string Action { get; set; }
    }

    public class ActionPlan
    {
        public string Name { get; set; }
        public string[] Steps { get; set; }
    }

    public class MonitoringCoverage
    {
        public double OverallCoverage { get; set; }
        public ComponentCoverage[] ComponentCoverage { get; set; }
    }

    public class ComponentCoverage
    {
        public string ComponentName { get; set; }
        public double Coverage { get; set; }
    }

    public class AlertConfiguration
    {
        public AlertChannel[] Channels { get; set; }
        public AlertRule[] Rules { get; set; }
    }

    public class AlertChannel
    {
        public string Type { get; set; }
        public string Destination { get; set; }
    }

    public class AlertRule
    {
        public string Name { get; set; }
        public string Condition { get; set; }
        public string Severity { get; set; }
    }

    public class IntegrationPlan
    {
        public IntegrationStep[] Steps { get; set; }
    }

    public class IntegrationStep
    {
        public int Order { get; set; }
        public string Description { get; set; }
    }

    public class CostBenefitAnalysis
    {
        public decimal EstimatedCost { get; set; }
        public decimal EstimatedBenefit { get; set; }
        public decimal Roi { get; set; }
    }
}
