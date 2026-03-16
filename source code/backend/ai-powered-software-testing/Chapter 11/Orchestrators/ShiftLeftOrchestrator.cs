
// Orchestrators/ShiftLeftOrchestrator.cs
using Chapter_11.Interfaces;
using Chapter_11.Models.Requests;
using Chapter_11.Models.Responses;
using Chapter_11.Services;


namespace Chapter_11.Orchestrators
{
    public class ShiftLeftOrchestrator : IShiftLeftOrchestrator
    {
        private readonly ILLMService _llmService;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<ShiftLeftOrchestrator> _logger;

        public ShiftLeftOrchestrator(
            ILLMService llmService,
            IDatabaseService databaseService,
            ILogger<ShiftLeftOrchestrator> logger)
        {
            _llmService = llmService;
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<AcceptanceCriteria[]> CoCreateAcceptanceCriteriaAsync(
            RequirementCollection requirements,
            CollaborationMode mode)
        {
            try
            {
                _logger.LogInformation("Co-creating acceptance criteria for {Count} requirements",
                    requirements.Items.Length);

                var criteriaList = new List<AcceptanceCriteria>();

                foreach (var requirement in requirements.Items)
                {
                    var prompt = $"Generate acceptance criteria for requirement: {requirement.Description} using {mode} collaboration mode";
                    var generatedCriteria = await _llmService.GenerateCompletionAsync(prompt);

                    criteriaList.Add(new AcceptanceCriteria
                    {
                        Id = Guid.NewGuid().ToString(),
                        RequirementId = requirement.Id,
                        Criterion = generatedCriteria,
                        IsAutomated = true
                    });
                }

                // Save to database
                foreach (var criterion in criteriaList)
                {
                    await _databaseService.SaveAsync(criterion);
                }

                return criteriaList.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to co-create acceptance criteria");
                throw;
            }
        }

        public async Task<TestScenario[]> GenerateTestScenariosAsync(
            RequirementCollection requirements,
            DesignDocument[] designDocuments,
            int shiftDepth)
        {
            try
            {
                _logger.LogInformation("Generating test scenarios at depth {ShiftDepth}", shiftDepth);

                var scenarios = new List<TestScenario>();

                foreach (var requirement in requirements.Items)
                {
                    var designContext = string.Join(", ", designDocuments?.Select(d => d.Name) ?? Array.Empty<string>());
                    var prompt = $"Generate test scenarios for requirement: {requirement.Description} with design context: {designContext} at shift depth {shiftDepth}";

                    var generatedScenarios = await _llmService.GenerateCompletionAsync(prompt);

                    scenarios.Add(new TestScenario
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"Test for {requirement.Id}",
                        Steps = new[] { "Step 1: Setup", "Step 2: Execute", "Step 3: Verify" },
                        ExpectedOutcome = "Success",
                        Tags = new[] { "shift-left", $"depth-{shiftDepth}" }
                    });
                }

                return scenarios.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate test scenarios");
                throw;
            }
        }

        public async Task<TestDataRequirement[]> DefineTestDataRequirementsAsync(
            TestScenario[] testScenarios,
            RequirementCollection requirements)
        {
            try
            {
                _logger.LogInformation("Defining test data requirements for {Count} scenarios",
                    testScenarios?.Length ?? 0);

                var dataRequirements = new List<TestDataRequirement>();

                if (testScenarios != null)
                {
                    foreach (var scenario in testScenarios)
                    {
                        // Use the requirements parameter here (no conflict now)
                        var relatedRequirement = requirements?.Items?.FirstOrDefault(r =>
                            scenario.Name?.Contains(r.Id) == true);

                        var prompt = $"Generate test data requirements for test scenario: {scenario.Name} based on requirement: {relatedRequirement?.Description ?? "unknown"}";
                        var generatedDataReq = await _llmService.GenerateCompletionAsync(prompt);

                        dataRequirements.Add(new TestDataRequirement
                        {
                            Id = Guid.NewGuid().ToString(),
                            TestScenarioId = scenario.Id,
                            DataType = DetermineDataType(scenario),
                            SampleData = new { description = generatedDataReq }
                        });
                    }
                }

                // Save to database
                foreach (var requirement in dataRequirements)
                {
                    await _databaseService.SaveAsync(requirement);
                }

                return dataRequirements.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to define test data requirements");
                throw;
            }
        }

        public async Task<RiskAssessment> AssessRisksAsync(
            RequirementCollection requirements,
            TestScenario[] testScenarios)
        {
            try
            {
                _logger.LogInformation("Assessing risks for {RequirementCount} requirements and {ScenarioCount} scenarios",
                    requirements?.Items?.Length ?? 0, testScenarios?.Length ?? 0);

                var prompt = $"Perform risk assessment for requirements and test scenarios. Requirements: {string.Join(", ", requirements?.Items?.Select(r => r.Description) ?? Array.Empty<string>())}";
                var riskAnalysis = await _llmService.GenerateCompletionAsync(prompt);

                var riskAssessment = new RiskAssessment
                {
                    Id = Guid.NewGuid().ToString(),
                    HighRisks = new RiskItem[0],
                    MediumRisks = new RiskItem[0],
                    LowRisks = new RiskItem[0]
                };

                // Save to database
                await _databaseService.SaveAsync(riskAssessment);

                return riskAssessment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assess risks");
                throw;
            }
        }

        private string DetermineDataType(TestScenario scenario)
        {
            if (scenario.Name?.Contains("performance") == true)
                return "Performance";
            if (scenario.Name?.Contains("security") == true)
                return "Security";
            if (scenario.Name?.Contains("integration") == true)
                return "Integration";
            return "Functional";
        }
    }
}
