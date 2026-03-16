using Chapter_1.Exceptions;
using Chapter_1.Models;
using Chapter_1.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Chapter_1.Services
{
    public class DeepSeekService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeepSeekService> _logger;

        public string ProviderName => "deepseek";

        public DeepSeekService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<DeepSeekService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("DeepSeek");
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateTestCodeAsync(string prompt, string context)
        {
            try
            {
                _logger.LogInformation("Generating test code with DeepSeek for prompt: {Prompt}", prompt.Substring(0, Math.Min(50, prompt.Length)));

                var requestBody = new
                {
                    model = "deepseek-coder",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a code generation expert specialized in test automation." },
                        new { role = "user", content = $"{context}\n\n{prompt}" }
                    },
                    temperature = 0.3,
                    max_tokens = 4096
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("", content);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseBody);

                // DeepSeek API response structure
                var result = jsonResponse.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test code with DeepSeek");
                throw new LLMCoordinationException($"DeepSeek generation failed: {ex.Message}", ProviderName);
            }
        }

        public bool CanHandleProvider(string providerName)
        {
            return providerName?.ToLower() == "deepseek";
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}