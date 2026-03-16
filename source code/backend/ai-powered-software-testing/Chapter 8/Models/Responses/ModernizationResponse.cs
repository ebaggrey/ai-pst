

namespace Chapter_8.Models.Responses
{
    // Models/Responses/ModernizationResponse.cs
    
        public class ModernizationResponse
        {
            public string RoadmapId { get; set; }
            public string ModernizationApproach { get; set; }
            public ModernizationRoadmap Roadmap { get; set; }
            public RiskAssessment RiskAssessment { get; set; }
            public ImplementationPlan ImplementationPlan { get; set; }
            public SuccessMetrics SuccessMetrics { get; set; }
            public StakeholderCommunication StakeholderCommunication { get; set; }
            public MonitoringPlan MonitoringPlan { get; set; }
            public ContingencyPlan[] ContingencyPlans { get; set; }
      
    }

        public class ModernizationRoadmap
        {
            public RoadmapPhase[] Phases { get; set; }
            public string TotalDuration { get; set; }
            public string[] Milestones { get; set; }
            public Dictionary<string, string> Dependencies { get; set; }
        }

        public class RoadmapPhase
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Duration { get; set; }
            public string[] Steps { get; set; }
            public string[] Deliverables { get; set; }
        }

        public class RiskAssessment
        {
            public Risk[] IdentifiedRisks { get; set; }
            public string OverallRiskLevel { get; set; }
            public MitigationPlan[] MitigationPlans { get; set; }
        }

        public class Risk
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Likelihood { get; set; }
            public string Impact { get; set; }
        }

        public class MitigationPlan
        {
            public string RiskName { get; set; }
            public string Strategy { get; set; }
            public string[] Actions { get; set; }
        }

        public class ImplementationPlan
        {
            public Task[] Tasks { get; set; }
            public string[] ResourceRequirements { get; set; }
            public Dictionary<string, string> Dependencies { get; set; }
        }

        public class Task
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string AssignedTo { get; set; }
            public string EstimatedHours { get; set; }
            public string[] Prerequisites { get; set; }
        }

        public class StakeholderCommunication
        {
            public string[] Stakeholders { get; set; }
            public CommunicationPlan[] CommunicationPlans { get; set; }
            public string[] KeyMessages { get; set; }
        }

        public class CommunicationPlan
        {
            public string Audience { get; set; }
            public string Frequency { get; set; }
            public string Format { get; set; }
            public string Content { get; set; }
        }

        public class MonitoringPlan
        {
            public Metric[] Metrics { get; set; }
            public string[] AlertRules { get; set; }
            public string ReviewFrequency { get; set; }
        }

        public class Metric
        {
            public string Name { get; set; }
            public string Target { get; set; }
            public string MeasurementMethod { get; set; }
        }

        public class ContingencyPlan
        {
            public string Trigger { get; set; }
            public string[] Actions { get; set; }
            public string Owner { get; set; }
        }
    
}
