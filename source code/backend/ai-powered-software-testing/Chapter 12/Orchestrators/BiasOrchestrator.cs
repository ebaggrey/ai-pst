
// Orchestrators/BiasOrchestrator.cs

using Chapter_12.Exceptions;
using Chapter_12.Interfaces;
using Chapter_12.Models.Requests;
using Chapter_12.Models.Responses;
using System.Text.Json;
using InvalidDataException = Chapter_12.Exceptions.InvalidDataException;

namespace Chapter_12.Orchestrators
{
    public class BiasOrchestrator : IBiasOrchestrator
    {
        private readonly ILLMService _llmService;
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<BiasOrchestrator> _logger;

        public BiasOrchestrator(
            ILLMService llmService,
            IAuditRepository auditRepository,
            ILogger<BiasOrchestrator> logger)
        {
            _llmService = llmService;
            _auditRepository = auditRepository;
            _logger = logger;
        }

        public async Task<BiasAuditResponse> AuditDataAsync(TestDataBiasAuditRequest request, string prompt)
        {
            try
            {
                // Validate data format
                if (!IsValidDataSample(request.DataSample))
                {
                    throw new InvalidDataException(
                        "Data sample format is invalid",
                        "DataSample",
                        "JSON array of objects"
                    );
                }

                // Build the comprehensive prompt for the LLM
                var fullPrompt = BuildBiasAuditPrompt(request, prompt);

                // Call LLM service
                var llmResponse = await _llmService.GenerateCompletionAsync(
                    fullPrompt,
                    request.SuggestionCount * 100, // Rough token estimate
                    new Dictionary<string, object>
                    {
                        ["temperature"] = 0.7,
                        ["top_p"] = 0.9
                    });

                // Parse the LLM response
                var auditResponse = ParseLLMResponse(llmResponse, request);

                // Save to database
                await _auditRepository.SaveAuditRecordAsync(auditResponse, "Completed");

                return auditResponse;
            }
            catch (AIServiceException)
            {
                // Re-throw AIServiceException to be handled by controller
                throw;
            }
            catch (InvalidDataException)
            {
                // Re-throw InvalidDataException to be handled by controller
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during bias audit for dataset {DatasetName}",
                    request.DatasetName);

                // Wrap unexpected exceptions
                throw new AIServiceException(
                    "An unexpected error occurred during bias audit",
                    "BiasOrchestrator",
                    500);
            }
        }

        private string BuildBiasAuditPrompt(TestDataBiasAuditRequest request, string basePrompt)
        {
            var dataSampleJson = JsonSerializer.Serialize(request.DataSample);

            return $@"
{basePrompt}

DATASET CONTEXT: {request.DataContext}
DATASET NAME: {request.DatasetName}
NUMBER OF SUGGESTIONS NEEDED: {request.SuggestionCount}

DATA SAMPLE:
{dataSampleJson}

Please analyze this data for bias and provide:
1. Bias findings with severity scores (0-1)
2. Inclusive suggestions for biased terms/patterns
3. Overall bias scores across different categories (gender, race, age, culture)
4. A summary risk level (Low/Medium/High/Critical)

Return the response in JSON format with the following structure:
{{
    ""findings"": [
        {{
            ""fieldName"": ""string"",
            ""biasType"": ""string"",
            ""description"": ""string"",
            ""severityScore"": 0.0,
            ""examples"": [""string""]
        }}
    ],
    ""suggestions"": [
        {{
            ""originalValue"": ""string"",
            ""suggestedValue"": ""string"",
            ""fieldName"": ""string"",
            ""rationale"": ""string"",
            ""confidenceScore"": 0
        }}
    ],
    ""overallBiasScore"": {{
        ""overallScore"": 0.0,
        ""genderBiasScore"": 0.0,
        ""racialBiasScore"": 0.0,
        ""ageBiasScore"": 0.0,
        ""culturalBiasScore"": 0.0,
        ""riskLevel"": ""string""
    }},
    ""metadata"": {{
        ""analysisTime"": ""string""
    }}
}}";
        }

        private BiasAuditResponse ParseLLMResponse(string llmResponse, TestDataBiasAuditRequest request)
        {
            try
            {
                // Try to extract JSON from the response
                var jsonStart = llmResponse.IndexOf('{');
                var jsonEnd = llmResponse.LastIndexOf('}');

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonResponse = llmResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    var parsed = JsonSerializer.Deserialize<BiasAuditResponse>(jsonResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (parsed != null)
                    {
                        parsed.DatasetName = request.DatasetName;
                        parsed.AuditId = Guid.NewGuid().ToString();
                        parsed.AuditDate = DateTime.UtcNow;

                        // Ensure metadata exists
                        parsed.Metadata ??= new Dictionary<string, object>();
                        parsed.Metadata["rowCount"] = request.DataSample.Count;
                        parsed.Metadata["dataContext"] = request.DataContext;

                        return parsed;
                    }
                }

                // If JSON parsing fails, create a fallback response
                return CreateFallbackResponse(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse LLM response");
                return CreateFallbackResponse(request);
            }
        }

        private BiasAuditResponse CreateFallbackResponse(TestDataBiasAuditRequest request)
        {
            return new BiasAuditResponse
            {
                DatasetName = request.DatasetName,
                AuditId = Guid.NewGuid().ToString(),
                AuditDate = DateTime.UtcNow,
                Findings = new List<BiasFinding>(),
                Suggestions = new List<InclusiveSuggestion>(),
                OverallBiasScore = new BiasScore
                {
                    OverallScore = 0.5,
                    GenderBiasScore = 0.5,
                    RacialBiasScore = 0.5,
                    AgeBiasScore = 0.5,
                    CulturalBiasScore = 0.5,
                    RiskLevel = "Medium"
                },
                Metadata = new Dictionary<string, object>
                {
                    ["rowCount"] = request.DataSample.Count,
                    ["dataContext"] = request.DataContext,
                    ["parseError"] = "Could not parse LLM response"
                }
            };
        }

        private bool IsValidDataSample(List<Dictionary<string, object>> dataSample)
        {
            if (dataSample == null || dataSample.Count == 0)
                return false;

            // Check that each item is a non-null dictionary
            foreach (var item in dataSample)
            {
                if (item == null)
                    return false;
            }

            return true;
        }
    }
}
