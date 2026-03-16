
using Chapter_8.Interfaces;
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;
using Chapter_8.Orchestrators;
using Chapter_8.Services.LLM;


namespace LegacyConquest.Orchestrators
{
    public class LegacyAnalysisOrchestrator : ILegacyAnalysisOrchestrator
    {
        private readonly ILegacyAnalyzer _legacyAnalyzer;
        private readonly ILLMService _llmService;
        private readonly ILogger<LegacyAnalysisOrchestrator> _logger;

        public LegacyAnalysisOrchestrator(
            ILegacyAnalyzer legacyAnalyzer,
            ILLMService llmService,
            ILogger<LegacyAnalysisOrchestrator> logger)
        {
            _legacyAnalyzer = legacyAnalyzer;
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<LegacyAnalysisResponse> AnalyzeLegacyCodebaseAsync(LegacyAnalysisRequest request)
        {
            _logger.LogInformation("Starting orchestrated analysis for codebase: {CodebaseName}",
                request.Codebase?.Name);

            // Perform deep analysis of legacy code
            var analysis = await _legacyAnalyzer.AnalyzeCodebaseAsync(request);

            // Use LLM to map business logic to code implementation
            var businessLogicMap = await MapBusinessLogicWithLLMAsync(analysis, request.BusinessContext);

            // Use LLM to identify risk hotspots
            var riskHotspots = await IdentifyRiskHotspotsWithLLMAsync(analysis, request.SafetyLevel);

            // Use LLM to uncover hidden assumptions
            var hiddenAssumptions = await UncoverHiddenAssumptionsWithLLMAsync(analysis, request.BusinessContext);

            // Assess modernization readiness
            var readiness = await AssessModernizationReadinessAsync(analysis, request.Codebase);

            // Use LLM to generate recommendations
            var recommendedActions = await GenerateRecommendedActionsWithLLMAsync(analysis, riskHotspots, readiness);

            // Use LLM to calculate confidence scores
            var confidenceScores = await CalculateConfidenceScoresWithLLMAsync(analysis, businessLogicMap);

            var response = new LegacyAnalysisResponse
            {
                AnalysisId = Guid.NewGuid().ToString(),
                CodebaseSummary = CreateCodebaseSummary(analysis, request.Codebase),
                BusinessLogicMap = businessLogicMap,
                RiskHotspots = riskHotspots,
                HiddenAssumptions = hiddenAssumptions,
                ModernizationReadiness = readiness,
                RecommendedActions = recommendedActions,
                ConfidenceScores = confidenceScores,
                NextSteps = DetermineNextSteps(analysis, request.FocusAreas)
            };

            return response;
        }

        private async Task<BusinessLogicMap[]> MapBusinessLogicWithLLMAsync(CodebaseAnalysis analysis, BusinessContext context)
        {
            var prompt = $@"
            Map the following business flows to code locations:
            Business Flows: {System.Text.Json.JsonSerializer.Serialize(context?.CriticalFlows)}
            Code Structure: {System.Text.Json.JsonSerializer.Serialize(analysis?.Structure)}
            
            Return as JSON array with BusinessFlowId, BusinessFlowDescription, CodeLocations, MappingConfidence.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<BusinessLogicMap[]>(prompt);
            return llmResponse ?? Array.Empty<BusinessLogicMap>();
        }

        private async Task<RiskHotspot[]> IdentifyRiskHotspotsWithLLMAsync(CodebaseAnalysis analysis, string safetyLevel)
        {
            var prompt = $@"
            Identify risk hotspots in this codebase with safety level {safetyLevel}:
            Code Smells: {System.Text.Json.JsonSerializer.Serialize(analysis?.CodeSmells)}
            Complexity: {System.Text.Json.JsonSerializer.Serialize(analysis?.Complexity)}
            
            Return as JSON array with Location, RiskType, Severity (1-10), Description, MitigationStrategies.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<RiskHotspot[]>(prompt);
            return llmResponse ?? Array.Empty<RiskHotspot>();
        }

        private async Task<HiddenAssumption[]> UncoverHiddenAssumptionsWithLLMAsync(CodebaseAnalysis analysis, BusinessContext context)
        {
            var prompt = $@"
            Uncover hidden assumptions in this legacy codebase:
            Code Structure: {System.Text.Json.JsonSerializer.Serialize(analysis?.Structure)}
            Business Context: {System.Text.Json.JsonSerializer.Serialize(context)}
            
            Return as JSON array with Description, Location, Impact, IsValidated.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<HiddenAssumption[]>(prompt);
            return llmResponse ?? Array.Empty<HiddenAssumption>();
        }

        private async Task<RecommendedAction[]> GenerateRecommendedActionsWithLLMAsync(
            CodebaseAnalysis analysis,
            RiskHotspot[] riskHotspots,
            ModernizationReadiness readiness)
        {
            var prompt = $@"
            Generate recommended actions based on:
            Risk Hotspots: {System.Text.Json.JsonSerializer.Serialize(riskHotspots)}
            Modernization Readiness: {System.Text.Json.JsonSerializer.Serialize(readiness)}
            
            Return as JSON array with Title, Description, Priority (1-10), EstimatedEffort, Dependencies.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<RecommendedAction[]>(prompt);
            return llmResponse ?? Array.Empty<RecommendedAction>();
        }

        private async Task<ConfidenceScore[]> CalculateConfidenceScoresWithLLMAsync(
            CodebaseAnalysis analysis,
            BusinessLogicMap[] businessLogicMaps)
        {
            var prompt = $@"
            Calculate confidence scores for the analysis:
            Business Logic Maps: {System.Text.Json.JsonSerializer.Serialize(businessLogicMaps)}
            Code Complexity: {System.Text.Json.JsonSerializer.Serialize(analysis?.Complexity)}
            
            Return as JSON array with Metric, Score (0-1), Explanation.
            ";

            var llmResponse = await _llmService.GenerateStructuredContentAsync<ConfidenceScore[]>(prompt);
            return llmResponse ?? Array.Empty<ConfidenceScore>();
        }

        private CodebaseSummary CreateCodebaseSummary(CodebaseAnalysis analysis, CodebaseInfo codebase)
        {
            return new CodebaseSummary
            {
                Name = codebase?.Name ?? "Unknown",
                TotalLines = codebase?.TotalLines ?? 0,
                ComplexityScore = analysis?.Complexity?.AverageCyclomaticComplexity != null
                    ? (int)analysis.Complexity.AverageCyclomaticComplexity
                    : 0,
                PrimaryTechnologies = codebase?.TechnologyStack ?? Array.Empty<string>(),
                DependencyCount = codebase?.Dependencies?.Length ?? 0,
                TechnicalDebtEstimate = analysis?.Complexity?.Metrics != null &&
                    analysis.Complexity.Metrics.ContainsKey("technicalDebtRatio")
                    ? analysis.Complexity.Metrics["technicalDebtRatio"]
                    : 0.0
            };
        }

        private NextStep[] DetermineNextSteps(CodebaseAnalysis analysis, string[] focusAreas)
        {
            var nextSteps = new List<NextStep>();

            if (focusAreas?.Contains("wrappers") == true)
            {
                nextSteps.Add(new NextStep
                {
                    Step = "Generate safety wrappers for high-risk modules",
                    Owner = "Development Team",
                    Timeline = "2 weeks"
                });
            }

            if (focusAreas?.Contains("tests") == true)
            {
                nextSteps.Add(new NextStep
                {
                    Step = "Create characterization tests for core business flows",
                    Owner = "QA Team",
                    Timeline = "3 weeks"
                });
            }

            nextSteps.Add(new NextStep
            {
                Step = "Review analysis with stakeholders",
                Owner = "Technical Lead",
                Timeline = "1 week"
            });

            return nextSteps.ToArray();
        }

        private async Task<ModernizationReadiness> AssessModernizationReadinessAsync(CodebaseAnalysis analysis, CodebaseInfo codebase)
        {
            return new ModernizationReadiness
            {
                ReadinessScore = CalculateReadinessScore(analysis, codebase),
                Strengths = new[] { "Well-structured core modules", "Good test coverage in some areas" },
                Weaknesses = new[] { "High coupling", "Technical debt in UI layer" },
                Opportunities = new[] { "Microservices extraction", "Cloud migration" },
                Threats = new[] { "Business critical systems", "Limited documentation" }
            };
        }

        private double CalculateReadinessScore(CodebaseAnalysis analysis, CodebaseInfo codebase)
        {
            var score = 0.5; // Base score

            if (analysis?.Complexity?.AverageCyclomaticComplexity < 10)
                score += 0.2;

            if (codebase?.Dependencies?.Length < 20)
                score += 0.1;

            if (analysis?.CodeSmells?.Length < 10)
                score += 0.2;

            return Math.Min(1.0, score);
        }
    }
}
