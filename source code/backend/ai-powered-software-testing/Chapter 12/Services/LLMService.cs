

// Services/LLMService.cs

using Chapter_12.Configuration;
using Chapter_12.Exceptions;
using Chapter_12.Interfaces;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Chapter_12.Services
{
    public class LLMService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly AIServiceConfig _config;
        private readonly ILogger<LLMService> _logger;

        public LLMService(
            HttpClient httpClient,
            IOptions<AIServiceConfig> config,
            ILogger<LLMService> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);

            if (!string.IsNullOrEmpty(_config.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
            }
        }

        public async Task<string> GenerateCompletionAsync(string prompt, int maxTokens, Dictionary<string, object> parameters = null)
        {
            try
            {
                var requestParams = parameters ?? _config.DefaultParameters ?? new Dictionary<string, object>();
                requestParams["prompt"] = prompt;
                requestParams["max_tokens"] = maxTokens;
                requestParams["model"] = _config.ModelName;

                var content = new StringContent(
                    JsonSerializer.Serialize(requestParams),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/v1/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("LLM service returned {StatusCode}: {Error}",
                        response.StatusCode, errorContent);

                    throw new AIServiceException(
                        $"LLM service returned {response.StatusCode}",
                        "LLMService",
                        (int)response.StatusCode);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;

                // Extract completion text - adjust based on actual API response format
                if (root.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("text", out var text))
                    {
                        return text.GetString();
                    }
                    else if (firstChoice.TryGetProperty("message", out var message) &&
                             message.TryGetProperty("content", out var content_text))
                    {
                        return content_text.GetString();
                    }
                }

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling LLM service");
                throw new AIServiceException("Failed to communicate with LLM service", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "LLM service timeout");
                throw new AIServiceException("LLM service timeout", "LLMService", 408);
            }
        }

        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
