using Chapter_6.Interfaces.BDDSupercharged.Interfaces;
using Chapter_6.Models.Requests;
using Chapter_6.Models.Responses;


namespace Chapter_6.Interfaces.Implementations
{
  
        public class BDDConversationOrchestrator : IBDDConversationOrchestrator
        {
            private readonly ILogger<BDDConversationOrchestrator> _logger;

            public BDDConversationOrchestrator(ILogger<BDDConversationOrchestrator> logger)
            {
                _logger = logger;
            }

            public async Task<BDDConversation> StartConversationAsync(BDCCoCreationRequest request)
            {
                _logger.LogInformation("Starting conversation for requirement: {Requirement}",
                    request.Requirement[..Math.Min(50, request.Requirement.Length)]);

                var participants = request.StakeholderPerspectives
                    .Select(sp => sp.Role)
                    .ToArray();

                var topics = ExtractTopicsFromRequirement(request.Requirement);

                return new BDDConversation
                {
                    Id = Guid.NewGuid().ToString(),
                    Participants = participants,
                    Topics = topics,
                    CreatedAt = DateTime.UtcNow
                };
            }

            public async Task<ConversationRound> FacilitateRoundAsync(BDDConversation conversation, int round, BDCCoCreationRequest request)
            {
                _logger.LogDebug("Facilitating round {Round} for conversation {ConversationId}",
                    round, conversation.Id);

                // Simulate stakeholder input based on perspectives
                var stakeholderInputs = GenerateStakeholderInputs(request.StakeholderPerspectives, round);
                var clarifications = GenerateClarifications(stakeholderInputs);
                var decisions = MakeDecisions(stakeholderInputs, clarifications);

                // Update conversation by creating a new instance
                var updatedConversation = new BDDConversation();

                // Copy all properties from original conversation
                updatedConversation.Id = conversation.Id;
                updatedConversation.Participants = conversation.Participants ?? Array.Empty<string>();
                updatedConversation.Topics = conversation.Topics ?? Array.Empty<string>();
                updatedConversation.CreatedAt = conversation.CreatedAt;
                updatedConversation.EndedAt = conversation.EndedAt;

                // Update decisions and open questions
                var existingDecisions = conversation.Decisions ?? Array.Empty<string>();
                updatedConversation.Decisions = existingDecisions.Concat(decisions).ToArray();
                updatedConversation.OpenQuestions = ExtractOpenQuestions(stakeholderInputs, clarifications);

                var consensusScore = CalculateConsensusScore(stakeholderInputs, decisions);

                return new ConversationRound
                {
                    RoundNumber = round,
                    StakeholderInputs = stakeholderInputs,
                    Clarifications = clarifications,
                    Decisions = decisions,
                    ConsensusScore = consensusScore,
                    UpdatedConversation = updatedConversation
                };
            }

            public async Task<BDDScenario[]> SynthesizeScenariosAsync(BDDConversation conversation)
            {
                _logger.LogInformation("Synthesizing scenarios from conversation {ConversationId}",
                    conversation.Id);

                var scenarios = new List<BDDScenario>();

                // Extract scenarios from conversation decisions
                foreach (var topic in conversation.Topics)
                {
                    var scenario = new BDDScenario
                    {
                        Title = $"Scenario for: {topic}",
                        Description = $"Generated from conversation on {conversation.CreatedAt:yyyy-MM-dd}",
                        Given = ExtractGivenStatements(conversation.Decisions, topic),
                        When = ExtractWhenStatements(conversation.Decisions, topic),
                        Then = ExtractThenStatements(conversation.Decisions, topic),
                        Tags = conversation.Participants.Concat(new[] { "generated" }).ToArray(),
                        Examples = ExtractExamples(conversation.Decisions, topic)
                    };

                    if (IsValidScenario(scenario))
                    {
                        scenarios.Add(scenario);
                    }
                }

                return scenarios.ToArray();
            }

            private string[] ExtractTopicsFromRequirement(string requirement)
            {
                // Simple topic extraction
                var keywords = new[] { "user", "system", "admin", "customer", "payment", "login", "registration" };
                return keywords
                    .Where(keyword => requirement.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }

            private string[] GenerateStakeholderInputs(StakeholderPerspective[] perspectives, int round)
            {
                var inputs = new List<string>();
                foreach (var perspective in perspectives)
                {
                    var input = $"Round {round + 1}: {perspective.Role} perspective - prioritizing {string.Join(", ", perspective.Priorities.Take(2))}";
                    inputs.Add(input);
                }
                return inputs.ToArray();
            }

            private string[] GenerateClarifications(string[] inputs)
            {
                return inputs.Select(input => $"Clarify: {input}").ToArray();
            }

            private string[] MakeDecisions(string[] inputs, string[] clarifications)
            {
                var decisions = new List<string>();
                for (int i = 0; i < Math.Min(inputs.Length, clarifications.Length); i++)
                {
                    decisions.Add($"Decision based on: {inputs[i]} and {clarifications[i]}");
                }
                return decisions.ToArray();
            }

            private string[] ExtractOpenQuestions(string[] inputs, string[] clarifications)
            {
                return inputs
                    .Where((_, i) => i % 2 == 0) // Every other input has open question
                    .Select(input => $"Open question about: {input}")
                    .ToArray();
            }

            private double CalculateConsensusScore(string[] inputs, string[] decisions)
            {
                if (inputs.Length == 0) return 0;
                return (double)decisions.Length / inputs.Length;
            }

            private string[] ExtractGivenStatements(string[] decisions, string topic)
            {
                return decisions
                    .Where(d => d.Contains("Given", StringComparison.OrdinalIgnoreCase) || d.Contains(topic, StringComparison.OrdinalIgnoreCase))
                    .Select(d => $"Given {d}")
                    .Take(3)
                    .ToArray();
            }

            private string[] ExtractWhenStatements(string[] decisions, string topic)
            {
                return decisions
                    .Where(d => d.Contains("When", StringComparison.OrdinalIgnoreCase) || d.Contains("action", StringComparison.OrdinalIgnoreCase))
                    .Select(d => $"When {d}")
                    .Take(2)
                    .ToArray();
            }

            private string[] ExtractThenStatements(string[] decisions, string topic)
            {
                return decisions
                    .Where(d => d.Contains("Then", StringComparison.OrdinalIgnoreCase) || d.Contains("outcome", StringComparison.OrdinalIgnoreCase))
                    .Select(d => $"Then {d}")
                    .Take(2)
                    .ToArray();
            }

            private string[] ExtractExamples(string[] decisions, string topic)
            {
                return decisions
                    .Where(d => d.Contains("example", StringComparison.OrdinalIgnoreCase) || d.Contains("scenario", StringComparison.OrdinalIgnoreCase))
                    .Take(2)
                    .ToArray();
            }

            private bool IsValidScenario(BDDScenario scenario)
            {
                return !string.IsNullOrWhiteSpace(scenario.Title) &&
                       scenario.Given.Length > 0 &&
                       scenario.When.Length > 0 &&
                       scenario.Then.Length > 0;
            }
        }
    
}
