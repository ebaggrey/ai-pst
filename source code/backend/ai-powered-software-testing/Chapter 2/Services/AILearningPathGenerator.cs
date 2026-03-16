using Chapter_2.Models;
using Chapter_2.Services.Interfaces;

namespace Chapter_2.Services
{
    public class AILearningPathGenerator : ILearningPathGenerator
    {
        private readonly ILLMServiceFactory _llmServiceFactory;
        private readonly ILogger<AILearningPathGenerator> _logger;

        public AILearningPathGenerator(
            ILLMServiceFactory llmServiceFactory,
            ILogger<AILearningPathGenerator> logger)
        {
            _llmServiceFactory = llmServiceFactory;
            _logger = logger;
        }

        public async Task<LearningPath> GenerateLearningPathAsync(LearningPathRequest request)
        {
            _logger.LogInformation("Generating learning path for {TargetRole}", request.TargetRole);

            var llmService = _llmServiceFactory.GetServiceForTask("learning-path", "comprehensive", "");

            var prompt = $@"
        Create a {request.TimelineDays}-day learning path for {request.TargetRole}.
        Focus on: {string.Join(", ", request.DesiredSkills)}
        Learning style: {request.LearningStyle}
        Hours per week: {request.HoursPerWeek}
        ";

            var result = await llmService.GenerateTestCodeAsync(prompt, "learning-path");

            return ParseLearningPath(result, request);
        }

        public async Task<LearningPath> AdjustLearningPathAsync(string pathId, string[] feedback)
        {
            _logger.LogInformation("Adjusting learning path {PathId}", pathId);

            // Implement adjustment logic
            return new LearningPath();
        }

        public async Task<ProgressReport> GetProgressAsync(string pathId)
        {
            return new ProgressReport
            {
                PathId = pathId,
                CompletionPercentage = 0.25m,
                CompletedMilestones = Array.Empty<Milestone>(),
                UpcomingMilestones = Array.Empty<Milestone>(),
                RecommendedResources = Array.Empty<Resource>()
            };
        }

        private LearningPath ParseLearningPath(string rawPath, LearningPathRequest request)
        {
            // Parse LLM response into structured LearningPath
            return new LearningPath
            {
                Phases = new[]
                {
                new Phase
                {
                    Name = "Foundation",
                    DurationDays = 14,
                    Topics = new[] { "Basic concepts", "Tool setup", "First tests" },
                    Activities = new[] { "Read documentation", "Complete tutorials", "Write simple tests" }
                }
            },
                Milestones = new[]
                {
                new Milestone
                {
                    Name = "First Test Suite",
                    Day = 7,
                    Description = "Create and run first automated test suite",
                    ValidationMethod = "Code review and execution"
                }
            },
                Resources = new[]
                {
                new Resource
                {
                    Type = "article",
                    Title = "Getting Started with Automated Testing",
                    Url = "https://example.com/testing-101",
                    EstimatedTime = "2 hours"
                }
            }
            };
        }
    }
}
