
using Chapter_8.Interfaces;
using Chapter_8.Models;
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;
using Chapter_8.Orchestrators;
using Chapter_8.Services.LLM;
using Task = Chapter_8.Models.Responses.Task;


namespace Chapter_8.Orchestrators
{
    public interface IModernizationPlanningOrchestrator
    {
        Task<ModernizationResponse> PlanModernizationAsync(RoadmapRequest request);
    }
}


namespace LegacyConquest.Orchestrators
{
    public class ModernizationPlanningOrchestrator : IModernizationPlanningOrchestrator
    {
        private readonly IModernizationPlanner _modernizationPlanner;
        private readonly ILLMService _llmService;
        private readonly ILogger<ModernizationPlanningOrchestrator> _logger;

        public ModernizationPlanningOrchestrator(
            IModernizationPlanner modernizationPlanner,
            ILLMService llmService,
            ILogger<ModernizationPlanningOrchestrator> logger)
        {
            _modernizationPlanner = modernizationPlanner;
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<ModernizationResponse> PlanModernizationAsync(RoadmapRequest request)
        {
            _logger.LogInformation("Starting orchestrated modernization planning with approach: {Approach}",
                request.ModernizationApproach);

            // Analyze modernization options
            var modernizationOptions = await AnalyzeModernizationOptionsWithLLMAsync(
                request.LegacyAnalysis, request.ModernizationApproach);

            // Create phased roadmap
            var roadmap = await _modernizationPlanner.CreateRoadmapAsync(
                modernizationOptions,
                request.BusinessPriorities ?? Array.Empty<BusinessPriority>(),
                request.Constraints ?? new PipelineConstraints(),
                request.SuccessMetrics ?? Array.Empty<SuccessMetric>());

            // Generate risk assessment
            var riskAssessment = await GenerateRiskAssessmentWithLLMAsync(roadmap, request.LegacyAnalysis);

            // Create implementation plan
            var implementationPlan = await CreateImplementationPlanWithLLMAsync(roadmap, request.Constraints);

            // Generate success metrics
            var successMetrics = await DefineSuccessMetricsWithLLMAsync(roadmap, request.SuccessMetrics);

            // Generate stakeholder communication
            var stakeholderCommunication = await GenerateStakeholderCommunicationWithLLMAsync(
                roadmap, request.BusinessPriorities);

            // Create monitoring plan
            var monitoringPlan = await CreateMonitoringPlanWithLLMAsync(roadmap, request.LegacyAnalysis);

            // Generate contingency plans
            var contingencyPlans = await GenerateContingencyPlansWithLLMAsync(roadmap, riskAssessment);

            var response = new ModernizationResponse
            {
                RoadmapId = Guid.NewGuid().ToString(),
                ModernizationApproach = request.ModernizationApproach ?? "incremental",
                Roadmap = roadmap,
                RiskAssessment = riskAssessment,
                ImplementationPlan = implementationPlan,
                SuccessMetrics = successMetrics,
                StakeholderCommunication = stakeholderCommunication,
                MonitoringPlan = monitoringPlan,
                ContingencyPlans = contingencyPlans
            };

            return response;
        }

        private async Task<ModernizationOption[]> AnalyzeModernizationOptionsWithLLMAsync(
            LegacyAnalysis analysis,
            string approach)
        {
            var prompt = $@"
            Analyze modernization options for this legacy system with approach {approach}:
            Analysis: {System.Text.Json.JsonSerializer.Serialize(analysis)}
            
            Return as JSON array with Name, Description, Effort (1-10), Impact (1-10), Risk (1-10), Prerequisites.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<ModernizationOption[]>(prompt);
            return llmResponse ?? new[]
            {
                new ModernizationOption
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Strangler Fig Pattern",
                    Description = "Incrementally replace legacy components",
                    Effort = 7,
                    Impact = 9,
                    Risk = 4,
                    Prerequisites = new[] { "Facade layer", "Characterization tests" }
                },
                new ModernizationOption
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Big Bang Rewrite",
                    Description = "Complete rewrite in modern stack",
                    Effort = 10,
                    Impact = 10,
                    Risk = 8,
                    Prerequisites = new[] { "Full requirements", "Parallel run capability" }
                }
            };
        }

        private async Task<RiskAssessment> GenerateRiskAssessmentWithLLMAsync(
            ModernizationRoadmap roadmap,
            LegacyAnalysis analysis)
        {
            var prompt = $@"
            Generate risk assessment for this modernization roadmap:
            Roadmap: {System.Text.Json.JsonSerializer.Serialize(roadmap)}
            Analysis: {System.Text.Json.JsonSerializer.Serialize(analysis)}
            
            Return as JSON with IdentifiedRisks (array with Name, Description, Likelihood, Impact), 
            OverallRiskLevel, MitigationPlans.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<RiskAssessment>(prompt);
            return llmResponse ?? new RiskAssessment
            {
                IdentifiedRisks = new[]
                {
                    new Risk { Name = "Data Migration", Description = "Data loss during migration", Likelihood = "Medium", Impact = "High" },
                    new Risk { Name = "Integration", Description = "Integration failures", Likelihood = "Medium", Impact = "Medium" }
                },
                OverallRiskLevel = "Medium",
                MitigationPlans = new[]
                {
                    new MitigationPlan { RiskName = "Data Migration", Strategy = "Backup and verify", Actions = new[] { "Full backup", "Validation scripts" } }
                }
            };
        }

        private async Task<ImplementationPlan> CreateImplementationPlanWithLLMAsync(
            ModernizationRoadmap roadmap,
            PipelineConstraints constraints)
        {
            var prompt = $@"
            Create implementation plan for this roadmap:
            Roadmap: {System.Text.Json.JsonSerializer.Serialize(roadmap)}
            Constraints: {System.Text.Json.JsonSerializer.Serialize(constraints)}
            
            Return as JSON with Tasks (array with Id, Description, AssignedTo, EstimatedHours, Prerequisites), 
            ResourceRequirements, Dependencies.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<ImplementationPlan>(prompt);
            return llmResponse ?? new ImplementationPlan
            {
                Tasks = new[]
                {
                    new Chapter_8.Models.Responses.Task { Id = "T1", Description = "Setup CI/CD", EstimatedHours = "40", Prerequisites = Array.Empty<string>() },
                    new Task { Id = "T2", Description = "Create facade", EstimatedHours = "80", Prerequisites = new[] { "T1" } }
                },
                ResourceRequirements = new[] { "2 Developers", "1 DevOps Engineer" },
                Dependencies = new Dictionary<string, string> { ["T2"] = "T1" }
            };
        }

        private async Task<SuccessMetrics> DefineSuccessMetricsWithLLMAsync(
            ModernizationRoadmap roadmap,
            SuccessMetric[] metrics)
        {
            var prompt = $@"
            Define success metrics for this modernization:
            Roadmap: {System.Text.Json.JsonSerializer.Serialize(roadmap)}
            Provided Metrics: {System.Text.Json.JsonSerializer.Serialize(metrics)}
            
            Return as JSON object with metrics (can be any structure).
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<SuccessMetrics>(prompt);
            return llmResponse ?? new SuccessMetrics
            {
                // This is a simplified version - in practice, you'd define proper metrics
            };
        }

        private async Task<StakeholderCommunication> GenerateStakeholderCommunicationWithLLMAsync(
            ModernizationRoadmap roadmap,
            BusinessPriority[] priorities)
        {
            var prompt = $@"
            Generate stakeholder communication plan:
            Roadmap: {System.Text.Json.JsonSerializer.Serialize(roadmap)}
            Business Priorities: {System.Text.Json.JsonSerializer.Serialize(priorities)}
            
            Return as JSON with Stakeholders, CommunicationPlans (array with Audience, Frequency, Format, Content), KeyMessages.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<StakeholderCommunication>(prompt);
            return llmResponse ?? new StakeholderCommunication
            {
                Stakeholders = new[] { "Business Owners", "Development Team", "Operations" },
                CommunicationPlans = new[]
                {
                    new CommunicationPlan
                    {
                        Audience = "Business Owners",
                        Frequency = "Monthly",
                        Format = "Executive Summary",
                        Content = "Progress and ROI"
                    }
                },
                KeyMessages = new[] { "Modernization reduces technical debt", "Incremental delivery of value" }
            };
        }

        private async Task<MonitoringPlan> CreateMonitoringPlanWithLLMAsync(
            ModernizationRoadmap roadmap,
            LegacyAnalysis analysis)
        {
            var prompt = $@"
            Create monitoring plan for modernization:
            Roadmap: {System.Text.Json.JsonSerializer.Serialize(roadmap)}
            Analysis: {System.Text.Json.JsonSerializer.Serialize(analysis)}
            
            Return as JSON with Metrics (array with Name, Target, MeasurementMethod), AlertRules, ReviewFrequency.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<MonitoringPlan>(prompt);
            return llmResponse ?? new MonitoringPlan
            {
                Metrics = new[]
                {
                    new Metric { Name = "System Health", Target = ">95%", MeasurementMethod = "Health checks" },
                    new Metric { Name = "Migration Progress", Target = "100%", MeasurementMethod = "Component count" }
                },
                AlertRules = new[] { "Alert if health < 90% for 5 minutes" },
                ReviewFrequency = "Weekly"
            };
        }

        private async Task<ContingencyPlan[]> GenerateContingencyPlansWithLLMAsync(
            ModernizationRoadmap roadmap,
            RiskAssessment riskAssessment)
        {
            var prompt = $@"
            Generate contingency plans:
            Roadmap: {System.Text.Json.JsonSerializer.Serialize(roadmap)}
            Risk Assessment: {System.Text.Json.JsonSerializer.Serialize(riskAssessment)}
            
            Return as JSON array with Trigger, Actions, Owner.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<ContingencyPlan[]>(prompt);
            return llmResponse ?? new[]
            {
                new ContingencyPlan
                {
                    Trigger = "Critical system failure",
                    Actions = new[] { "Rollback to legacy", "Notify stakeholders" },
                    Owner = "Operations Team"
                },
                new ContingencyPlan
                {
                    Trigger = "Migration delay > 2 weeks",
                    Actions = new[] { "Adjust timeline", "Add resources" },
                    Owner = "Project Manager"
                }
            };
        }
    }
}