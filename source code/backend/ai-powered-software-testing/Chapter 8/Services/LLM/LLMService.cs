
using System.Text;
using System.Text.Json;

namespace Chapter_8.Services.LLM
{
    public class LLMService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LLMService> _logger;
        private readonly string _apiKey;
        private readonly string _modelName;
        private readonly string _endpoint;

        public LLMService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<LLMService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var llmSettings = configuration.GetSection("LLMSettings");
            _apiKey = llmSettings["ApiKey"] ?? throw new InvalidOperationException("LLM API Key not configured");
            _modelName = llmSettings["ModelName"] ?? "gpt-4";
            _endpoint = llmSettings["Endpoint"] ?? "https://api.openai.com/v1/chat/completions";

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            try
            {
                _logger.LogDebug("Generating content with prompt length: {PromptLength}", prompt.Length);

                var requestBody = new
                {
                    model = _modelName,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a legacy code modernization expert. Provide detailed, accurate responses." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.2,
                    max_tokens = 4000
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(_endpoint, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var generatedText = jsonResponse.GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                _logger.LogDebug("Successfully generated content");

                return generatedText ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating content from LLM");

                // Fallback to simulated response for development
                if (ex is HttpRequestException || ex is JsonException)
                {
                    _logger.LogWarning("Using fallback response generation");
                    return GenerateFallbackContent(prompt);
                }

                throw;
            }
        }

        public async Task<T> GenerateStructuredContentAsync<T>(string prompt) where T : class
        {
            try
            {
                _logger.LogDebug("Generating structured content of type {TypeName}", typeof(T).Name);

                var enhancedPrompt = $@"
{prompt}

IMPORTANT: Return ONLY valid JSON without any additional text, markdown formatting, or explanation.
The response must be a valid JSON object or array that matches the requested structure.
";

                var response = await GenerateContentAsync(enhancedPrompt);

                // Clean the response - remove markdown code blocks if present
                var cleanedResponse = response
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var result = JsonSerializer.Deserialize<T>(cleanedResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogDebug("Successfully deserialized structured content");

                return result ?? throw new InvalidOperationException("Failed to deserialize LLM response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating structured content from LLM");

                // Return default instance for the type
                return Activator.CreateInstance<T>();
            }
        }

        private string GenerateFallbackContent(string prompt)
        {
            // Simple fallback for development when LLM is unavailable
            if (prompt.Contains("RiskHotspot"))
            {
                return JsonSerializer.Serialize(new[]
                {
                    new { Location = "PaymentProcessor.cs", RiskType = "Security", Severity = 8, Description = "SQL injection vulnerability" }
                });
            }

            return "{}";
        }
    }
}