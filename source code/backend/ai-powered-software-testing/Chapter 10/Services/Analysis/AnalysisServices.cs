
// Services/Analysis/ObjectiveAnalyzerService.cs
using Chapter_10.Analysis;
using Chapter_10.Exceptions;
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;

namespace Chapter_10.Services.Analysis
{
    public class ObjectiveAnalyzerService : IObjectiveAnalyzer
    {
        private readonly ILogger<ObjectiveAnalyzerService> _logger;
        private readonly IConfiguration _configuration;

        public ObjectiveAnalyzerService(ILogger<ObjectiveAnalyzerService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ObjectiveAnalysis> AnalyzeObjectivesAsync(string[] objectives)
        {
            _logger.LogInformation("Analyzing {Count} objectives", objectives.Length);

            var keyObjectives = new List<KeyObjective>();
            var ambiguousObjectives = new List<string>();
            var measurableAspects = new List<string>();

            foreach (var objective in objectives)
            {
                if (IsAmbiguous(objective))
                {
                    ambiguousObjectives.Add(objective);
                }
                else
                {
                    keyObjectives.Add(new KeyObjective
                    {
                        Name = objective,
                        TargetValue = ExtractTargetValue(objective),
                        Unit = ExtractUnit(objective),
                        TargetDate = DateTime.Now.AddMonths(3)
                    });

                    measurableAspects.AddRange(ExtractMeasurableAspects(objective));
                }
            }

            if (ambiguousObjectives.Any())
            {
                throw new ObjectiveAmbiguityException(
                    "Some objectives are too ambiguous for measurement",
                    ambiguousObjectives.ToArray(),
                    GenerateClarificationQuestions(ambiguousObjectives.ToArray()));
            }

            return new ObjectiveAnalysis
            {
                KeyObjectives = keyObjectives.ToArray(),
                MeasurableAspects = measurableAspects.Distinct().ToArray(),
                AmbiguousObjectives = ambiguousObjectives.ToArray()
            };
        }

        private bool IsAmbiguous(string objective)
        {
            var ambiguousTerms = new[] { "improve", "better", "good", "fast", "quality" };
            return ambiguousTerms.Any(term =>
                objective.Contains(term, StringComparison.OrdinalIgnoreCase)) &&
                !objective.Any(char.IsDigit);
        }

        private double ExtractTargetValue(string objective)
        {
            // Simple extraction logic - in real implementation, use regex or NLP
            var words = objective.Split(' ');
            foreach (var word in words)
            {
                if (double.TryParse(word, out double value))
                    return value;
            }
            return 100.0; // Default target
        }

        private string ExtractUnit(string objective)
        {
            if (objective.Contains("%", StringComparison.OrdinalIgnoreCase) ||
                objective.Contains("percent", StringComparison.OrdinalIgnoreCase))
                return "%";
            if (objective.Contains("ms", StringComparison.OrdinalIgnoreCase))
                return "ms";
            return "count";
        }

        private string[] ExtractMeasurableAspects(string objective)
        {
            var aspects = new List<string>();
            if (objective.Contains("performance", StringComparison.OrdinalIgnoreCase))
                aspects.Add("response time");
            if (objective.Contains("quality", StringComparison.OrdinalIgnoreCase))
                aspects.Add("defect rate");
            return aspects.ToArray();
        }

        private string[] GenerateClarificationQuestions(string[] ambiguousObjectives)
        {
            return new[]
            {
                "What specific metric would indicate success?",
                "What is the target value or range?",
                "By when should this objective be achieved?",
                "How will you measure progress?"
            };
        }
    }
}

// Services/Analysis/ActivityMapperService.cs
namespace Chapter_10.Services.Analysis
{
    public class ActivityMapperService : IActivityMapper
    {
        private readonly ILogger<ActivityMapperService> _logger;

        public ActivityMapperService(ILogger<ActivityMapperService> logger)
        {
            _logger = logger;
        }

        public async Task<ActivityValueMapping> MapActivitiesToValueAsync(string[] activities, string[] objectives)
        {
            _logger.LogInformation("Mapping {ActivityCount} activities to {ObjectiveCount} objectives",
                activities.Length, objectives.Length);

            var mappings = new List<ActivityValue>();

            foreach (var activity in activities)
            {
                var relatedObjectives = objectives
                    .Where(o => o.Contains(GetObjectiveCategory(activity), StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                mappings.Add(new ActivityValue
                {
                    Activity = activity,
                    RelatedObjectives = relatedObjectives,
                    ValueContribution = relatedObjectives.Any() ? 1.0 / relatedObjectives.Length : 0.1
                });
            }

            return new ActivityValueMapping
            {
                Mappings = mappings.ToArray()
            };
        }

        private string GetObjectiveCategory(string activity)
        {
            if (activity.Contains("test", StringComparison.OrdinalIgnoreCase))
                return "quality";
            if (activity.Contains("review", StringComparison.OrdinalIgnoreCase))
                return "process";
            return "general";
        }
    }
}

// Services/Analysis/RelationshipDefinerService.cs
namespace Chapter_10.Services.Analysis
{
    public class RelationshipDefinerService : IRelationshipDefiner
    {
        private readonly ILogger<RelationshipDefinerService> _logger;

        public RelationshipDefinerService(ILogger<RelationshipDefinerService> logger)
        {
            _logger = logger;
        }

        public async Task<MetricRelationship[]> DefineRelationshipsAsync(DesignedMetric[] metrics)
        {
            _logger.LogInformation("Defining relationships for {MetricCount} metrics", metrics.Length);

            var relationships = new List<MetricRelationship>();

            for (int i = 0; i < metrics.Length; i++)
            {
                for (int j = i + 1; j < metrics.Length; j++)
                {
                    // Simple relationship detection - in real implementation, use correlation analysis
                    if (metrics[i].Category == metrics[j].Category)
                    {
                        relationships.Add(new MetricRelationship
                        {
                            SourceMetricId = metrics[i].MetricId,
                            TargetMetricId = metrics[j].MetricId,
                            RelationshipType = "correlation",
                            Strength = 0.7
                        });
                    }
                }
            }

            return relationships.ToArray();
        }
    }
}

// Services/Analysis/CollectionPlannerService.cs
namespace Chapter_10.Services.Analysis
{
    public class CollectionPlannerService : ICollectionPlanner
    {
        private readonly ILogger<CollectionPlannerService> _logger;

        public CollectionPlannerService(ILogger<CollectionPlannerService> logger)
        {
            _logger = logger;
        }

        public async Task<CollectionPlan> CreateCollectionPlanAsync(
            DesignedMetric[] metrics,
            MetricConstraints constraints)
        {
            _logger.LogInformation("Creating collection plan for {MetricCount} metrics", metrics.Length);

            var steps = new List<string>();
            foreach (var metric in metrics)
            {
                steps.Add($"Collect {metric.Name} from {metric.DataSource ?? "primary source"}");
            }

            return new CollectionPlan
            {
                CollectionSteps = steps.ToArray(),
                Tools = new[] { "Data Pipeline", "ETL Jobs", "Quality Dashboard" },
                Frequency = constraints.RequiredDimensions?.Contains("daily") == true ? "daily" : "weekly",
                ResponsibleTeam = "Data Engineering"
            };
        }
    }
}

// Services/Analysis/InterpretationBuilderService.cs
namespace Chapter_10.Services.Analysis
{
    public class InterpretationBuilderService : IInterpretationBuilder
    {
        private readonly ILogger<InterpretationBuilderService> _logger;

        public InterpretationBuilderService(ILogger<InterpretationBuilderService> logger)
        {
            _logger = logger;
        }

        public async Task<InterpretationFramework> BuildInterpretationFrameworkAsync(
            DesignedMetric[] metrics,
            string[] objectives)
        {
            _logger.LogInformation("Building interpretation framework");

            var thresholds = metrics.Select(m => new ThresholdDefinition
            {
                MetricId = m.MetricId,
                Green = m.TargetValue * 0.9,
                Yellow = m.TargetValue * 0.7,
                Red = m.TargetValue * 0.5
            }).ToArray();

            return new InterpretationFramework
            {
                InterpretationRules = new[]
                {
                    "Values above green threshold indicate good performance",
                    "Values between yellow and green require attention",
                    "Values below red threshold need immediate action"
                },
                Thresholds = thresholds,
                OutlierHandling = new[]
                {
                    "Remove values beyond 3 standard deviations",
                    "Apply winsorization for extreme values"
                }
            };
        }
    }
}

// Services/Analysis/ValidationRuleCreatorService.cs
namespace Chapter_10.Services.Analysis
{
    public class ValidationRuleCreatorService : IValidationRuleCreator
    {
        private readonly ILogger<ValidationRuleCreatorService> _logger;

        public ValidationRuleCreatorService(ILogger<ValidationRuleCreatorService> logger)
        {
            _logger = logger;
        }

        public async Task<ValidationRule[]> CreateValidationRulesAsync(DesignedMetric[] metrics)
        {
            _logger.LogInformation("Creating validation rules for {MetricCount} metrics", metrics.Length);

            return metrics.Select(m => new ValidationRule
            {
                MetricId = m.MetricId,
                Rule = $"{m.Name} must be between 0 and 100",
                ErrorMessage = $"{m.Name} value is out of valid range"
            }).ToArray();
        }
    }
}

// Services/Analysis/ImplementationGuideGeneratorService.cs
namespace Chapter_10.Services.Analysis
{
    public class ImplementationGuideGeneratorService : IImplementationGuideGenerator
    {
        private readonly ILogger<ImplementationGuideGeneratorService> _logger;

        public ImplementationGuideGeneratorService(ILogger<ImplementationGuideGeneratorService> logger)
        {
            _logger = logger;
        }

        public async Task<ImplementationGuidance> GenerateImplementationGuidanceAsync(
            MetricDesignResult design,
            string[] activities)
        {
            _logger.LogInformation("Generating implementation guidance");

            return new ImplementationGuidance
            {
                Prerequisites = new[]
                {
                    "Set up data collection pipelines",
                    "Configure monitoring dashboards",
                    "Train team on metric interpretation"
                },
                Steps = new[]
                {
                    "Phase 1: Deploy collection mechanisms",
                    "Phase 2: Validate data quality",
                    "Phase 3: Train stakeholders",
                    "Phase 4: Go live with monitoring"
                },
                Pitfalls = new[]
                {
                    "Avoid measuring too many metrics",
                    "Ensure data consistency",
                    "Regularly review metric relevance"
                },
                SuccessFactors = new[]
                {
                    "Clear ownership",
                    "Automated collection",
                    "Regular review cycles"
                }
            };
        }
    }
}

// Services/Analysis/SuccessCriteriaDefinerService.cs
namespace Chapter_10.Services.Analysis
{
    public class SuccessCriteriaDefinerService : ISuccessCriteriaDefiner
    {
        private readonly ILogger<SuccessCriteriaDefinerService> _logger;

        public SuccessCriteriaDefinerService(ILogger<SuccessCriteriaDefinerService> logger)
        {
            _logger = logger;
        }

        public async Task<SuccessCriteria> DefineSuccessCriteriaAsync(
            MetricDesignResult design,
            string[] objectives)
        {
            _logger.LogInformation("Defining success criteria");

            return new SuccessCriteria
            {
                QuantitativeCriteria = new[]
                {
                    "95% data completeness",
                    "<5% measurement error",
                    "Monthly trend improvement"
                },
                QualitativeCriteria = new[]
                {
                    "Stakeholder satisfaction",
                    "Actionable insights generated",
                    "Decision impact"
                },
                ReviewPeriod = "Quarterly"
            };
        }
    }
}