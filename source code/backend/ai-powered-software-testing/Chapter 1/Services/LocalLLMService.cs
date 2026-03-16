using Chapter_1.Exceptions;
using Chapter_1.Models;
using Chapter_1.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Chapter_1.Services
{
    public class LocalLLMService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalLLMService> _logger;

        public string ProviderName => "llama";

        public LocalLLMService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<LocalLLMService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("LocalLLM");
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateTestCodeAsync(string prompt, string context)
        {
            try
            {
                _logger.LogInformation("Generating test code with Local LLM for prompt: {Prompt}", prompt.Substring(0, Math.Min(50, prompt.Length)));

                var model = _configuration["LLMProviders:LocalLLM:Model"] ?? "llama2-13b-chat";
                var endpoint = _configuration["LLMProviders:LocalLLM:Endpoint"] ?? "http://localhost:8080/v1/completions";

                var requestBody = new
                {
                    model = model,
                    prompt = $"{context}\n\n{prompt}\n\nGenerate test code:",
                    temperature = 0.7,
                    max_tokens = 2048,
                    top_p = 0.9
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseBody);

                string result = string.Empty;

                // Try different response formats
                if (jsonResponse.RootElement.TryGetProperty("choices", out var choices))
                {
                    result = choices[0].GetProperty("text").GetString() ?? string.Empty;
                }
                else if (jsonResponse.RootElement.TryGetProperty("response", out var responseProp))
                {
                    result = responseProp.GetString() ?? string.Empty;
                }
                else
                {
                    result = jsonResponse.RootElement.ToString();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating test code with Local LLM");
                throw new LLMCoordinationException($"Local LLM generation failed: {ex.Message}", ProviderName);
            }
        }

        public bool CanHandleProvider(string providerName)
        {
            return providerName?.ToLower() == "llama" ||
                   providerName?.ToLower() == "local" ||
                   providerName?.ToLower() == "local-llm";
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var endpoint = _configuration["LLMProviders:LocalLLM:Endpoint"] ?? "http://localhost:8080/v1/completions";
                var uri = new Uri(endpoint);
                var baseUri = uri.GetLeftPart(UriPartial.Authority);

                var response = await _httpClient.GetAsync(baseUri + "/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}