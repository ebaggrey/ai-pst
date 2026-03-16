
using Chapter_10.Analysis;
using Chapter_10.Data;
using Chapter_10.Interfaces;
using Chapter_10.Interfaces.LLM;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Orchestrators
{
    public class MetricAnalysisOrchestrator : IMetricAnalysisOrchestrator
    {
        private readonly IObjectiveAnalyzer _objectiveAnalyzer;
        private readonly IActivityMapper _activityMapper;
        private readonly IRelationshipDefiner _relationshipDefiner;
        private readonly ICollectionPlanner _collectionPlanner;
        private readonly IInterpretationBuilder _interpretationBuilder;
        private readonly IValidationRuleCreator _validationRuleCreator;
        private readonly IImplementationGuideGenerator _implementationGuideGenerator;
        private readonly ISuccessCriteriaDefiner _successCriteriaDefiner;
        private readonly ILLMService _llmService;
        private readonly IMetricsRepository _repository;
        private readonly ILogger<MetricAnalysisOrchestrator> _logger;

        public MetricAnalysisOrchestrator(
            IObjectiveAnalyzer objectiveAnalyzer,
            IActivityMapper activityMapper,
            IRelationshipDefiner relationshipDefiner,
            ICollectionPlanner collectionPlanner,
            IInterpretationBuilder interpretationBuilder,
            IValidationRuleCreator validationRuleCreator,
            IImplementationGuideGenerator implementationGuideGenerator,
            ISuccessCriteriaDefiner successCriteriaDefiner,
            ILLMService llmService,
            IMetricsRepository repository,
            ILogger<MetricAnalysisOrchestrator> logger)
        {
            _objectiveAnalyzer = objectiveAnalyzer;
            _activityMapper = activityMapper;
            _relationshipDefiner = relationshipDefiner;
            _collectionPlanner = collectionPlanner;
            _interpretationBuilder = interpretationBuilder;
            _validationRuleCreator = validationRuleCreator;
            _implementationGuideGenerator = implementationGuideGenerator;
            _successCriteriaDefiner = successCriteriaDefiner;
            _llmService = llmService;
            _repository = repository;
            _logger = logger;
        }

        public async Task<ObjectiveAnalysis> AnalyzeObjectivesForMeasurementAsync(string[] objectives)
        {
            _logger.LogInformation("Analyzing {Count} objectives for measurement", objectives.Length);

            // First, check if we have similar analyses in database
            var existingDesigns = await _repository.GetMetricDesignsAsync(
                from: DateTime.UtcNow.AddDays(-30));

            // Use LLM to enhance objective analysis
            var llmInsights = await _llmService.GenerateInsightAsync(
                $"Analyze these business objectives for measurability: {string.Join(", ", objectives)}",
                new InsightContext { PriorityLevel = "high" });

            _logger.LogDebug("LLM insights for objectives: {Insights}", llmInsights);

            var analysis = await _objectiveAnalyzer.AnalyzeObjectivesAsync(objectives);

            // Enhance analysis with LLM insights if needed
            if (analysis.AmbiguousObjectives?.Any() == true)
            {
                var clarificationPrompt = $"These objectives are ambiguous: {string.Join(", ", analysis.AmbiguousObjectives)}. Suggest how to make them SMART.";
                var suggestions = await _llmService.GenerateInsightAsync(clarificationPrompt, new InsightContext());
                _logger.LogInformation("LLM suggestions for ambiguous objectives: {Suggestions}", suggestions);
            }

            return analysis;
        }

        public async Task<ActivityValueMapping> MapActivitiesToValueAsync(
            string[] activities,
            string[] objectives)
        {
            _logger.LogInformation("Mapping {ActivityCount} activities to value", activities.Length);

            var mapping = await _activityMapper.MapActivitiesToValueAsync(activities, objectives);

            // Use LLM to identify hidden value connections
            var connectionPrompt = $"Identify hidden connections between activities {string.Join(", ", activities)} and objectives {string.Join(", ", objectives)}";
            var connections = await _llmService.GenerateInsightAsync(connectionPrompt, new InsightContext());

            _logger.LogDebug("LLM identified connections: {Connections}", connections);

            return mapping;
        }

        public async Task<MetricRelationship[]> DefineMetricRelationshipsAsync(DesignedMetric[] metrics)
        {
            _logger.LogInformation("Defining relationships for {MetricCount} metrics", metrics.Length);

            // Get statistical relationships
            var relationships = (await _relationshipDefiner.DefineRelationshipsAsync(metrics)).ToList();

            // Use LLM to identify semantic relationships
            var metricsDesc = metrics.Select(m => $"{m.Name}: {m.Description}").ToArray();
            var relationshipPrompt = $"Identify semantic relationships between these metrics: {string.Join("; ", metricsDesc)}";
            var semanticRelationships = await _llmService.GenerateInsightAsync(relationshipPrompt, new InsightContext());

            // Parse LLM response and add to relationships (simplified)
            if (!string.IsNullOrEmpty(semanticRelationships))
            {
                // In production, parse the LLM response properly
                relationships.Add(new MetricRelationship
                {
                    SourceMetricId = metrics.First().MetricId,
                    TargetMetricId = metrics.Last().MetricId,
                    RelationshipType = "semantic",
                    Strength = 0.6
                });
            }

            return relationships.ToArray();
        }

        public async Task<CollectionPlan> CreateCollectionPlanAsync(
            DesignedMetric[] metrics,
            MetricConstraints constraints)
        {
            _logger.LogInformation("Creating collection plan for {MetricCount} metrics", metrics.Length);

            // Check database for existing collection methods
            var existingMetrics = await _repository.GetAllMetricDefinitionsAsync();
            var knownMethods = existingMetrics
                .Where(m => metrics.Any(d => d.Name.Contains(m.Name)))
                .Select(m => m.CollectionMethod)
                .Distinct()
                .ToArray();

            var plan = await _collectionPlanner.CreateCollectionPlanAsync(metrics, constraints);

            // Enhance plan with known methods
            if (knownMethods.Any() && plan.Tools == null || !plan.Tools.Any())
            {
                plan.Tools = knownMethods;
            }

            return plan;
        }

        public async Task<InterpretationFramework> BuildInterpretationFrameworkAsync(
            DesignedMetric[] metrics,
            string[] objectives)
        {
            _logger.LogInformation("Building interpretation framework");

            var framework = await _interpretationBuilder.BuildInterpretationFrameworkAsync(metrics, objectives);

            // Use LLM to generate natural language interpretation rules
            foreach (var metric in metrics)
            {
                var explanation = await _llmService.GenerateNaturalLanguageExplanationAsync(
                    metric.Name,
                    metric.TargetValue,
                    string.Join(", ", objectives));

                _logger.LogDebug("LLM explanation for {MetricName}: {Explanation}", metric.Name, explanation);
            }

            return framework;
        }

        public async Task<ValidationRule[]> CreateValidationRulesAsync(DesignedMetric[] metrics)
        {
            _logger.LogInformation("Creating validation rules for {MetricCount} metrics", metrics.Length);

            var rules = await _validationRuleCreator.CreateValidationRulesAsync(metrics);

            // Validate rules with LLM
            var isValid = await _llmService.ValidateMetricDesignAsync(metrics, new[] { "valid", "measurable" });

            if (!isValid)
            {
                _logger.LogWarning("LLM validation suggests rules may need improvement");
            }

            return rules;
        }

        public async Task<ImplementationGuidance> GenerateImplementationGuidanceAsync(
            MetricDesignResult design,
            string[] activities)
        {
            _logger.LogInformation("Generating implementation guidance");

            var guidance = await _implementationGuideGenerator.GenerateImplementationGuidanceAsync(design, activities);

            // Save design to database
            var designEntity = new MetricDesignEntity
            {
                DesignId = Guid.NewGuid().ToString(),
                BusinessObjectives = design.Metrics?.SelectMany(m => m.BusinessObjectives ?? Array.Empty<string>()).Distinct().ToArray() ?? Array.Empty<string>(),
                Metrics = design.Metrics ?? Array.Empty<DesignedMetric>(),
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.SaveMetricDesignAsync(designEntity);

            return guidance;
        }

        public async Task<SuccessCriteria> DefineSuccessCriteriaAsync(
            MetricDesignResult design,
            string[] objectives)
        {
            _logger.LogInformation("Defining success criteria");

            var criteria = await _successCriteriaDefiner.DefineSuccessCriteriaAsync(design, objectives);

            // Enhance with LLM suggestions
            var prompt = $"Based on metrics design and objectives: {string.Join(", ", objectives)}, suggest success criteria";
            var suggestions = await _llmService.GenerateInsightAsync(prompt, new InsightContext());

            _logger.LogDebug("LLM success criteria suggestions: {Suggestions}", suggestions);

            return criteria;
        }
    }
}
