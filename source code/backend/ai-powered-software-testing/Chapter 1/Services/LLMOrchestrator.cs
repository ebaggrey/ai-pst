using Chapter_1.Exceptions;
using Chapter_1.Models;
using Chapter_1.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options;
namespace Chapter_1.Services
{
    
    

    
        public class LLMOrchestrator : ILLMOrchestrator
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly IConfiguration _config;
            private readonly ILogger<LLMOrchestrator> _logger;
            private readonly LLMStrategyOptions _strategyOptions;

            public LLMOrchestrator(
                IServiceProvider serviceProvider,
                IConfiguration config,
                ILogger<LLMOrchestrator> logger,
                IOptions<LLMStrategyOptions> strategyOptions)
            {
                _serviceProvider = serviceProvider;
                _config = config;
                _logger = logger;
                _strategyOptions = strategyOptions.Value;
            }

            public async Task<Dictionary<string, LLMResult>> OrchestrateAnalysisAsync(
                Dictionary<string, string> prompts)
            {
                var results = new Dictionary<string, LLMResult>();
                var tasks = new List<Task<KeyValuePair<string, LLMResult>>>();

                // Assign each prompt to the most appropriate LLM
                foreach (var (area, prompt) in prompts)
                {
                    var llmProvider = SelectBestLLMForArea(area);
                    var task = ProcessWithLLMAsync(area, prompt, llmProvider);
                    tasks.Add(task);
                }

                // Process in parallel
                var completedTasks = await Task.WhenAll(tasks);

                // Fix: Properly extract KeyValuePair into dictionary
                foreach (var kvp in completedTasks)
                {
                    results[kvp.Key] = kvp.Value;
                }

                return results;
            }

            public async Task<LLMResult> ProcessSinglePromptAsync(string area, string prompt, string? providerName = null)
            {
                var provider = providerName ?? SelectBestLLMForArea(area);
                // Fix: Extract just the Value from the KeyValuePair
                var result = await ProcessWithLLMAsync(area, prompt, provider);
                return result.Value;
            }

            private string SelectBestLLMForArea(string area)
            {
                // Check configuration for area mapping
                if (_strategyOptions.AreaMapping?.TryGetValue(area.ToLower(), out var configuredProvider) == true)
                {
                    return configuredProvider;
                }

                // Default mappings based on LLM strengths
                return area.ToLower() switch
                {
                    "security" => "claude",       // Careful analysis
                    "performance" => "gemini",     // Data interpretation
                    "integration" => "deepseek",   // Code/API understanding
                    "ui" => "chatgpt",            // User flow understanding
                    "api" => "deepseek",           // API testing expertise
                    "database" => "llama",         // General purpose
                    _ => "chatgpt"                   // Default
                };
            }

            private async Task<KeyValuePair<string, LLMResult>> ProcessWithLLMAsync(
                string area, string prompt, string providerName)
            {
                var startTime = DateTime.UtcNow;
                var attempt = 0;
                var maxRetries = _strategyOptions.MaxRetries;

                while (attempt <= maxRetries)
                {
                    try
                    {
                        var llmService = GetLLMService(providerName);

                        _logger.LogDebug("Processing {Area} with {Provider} (Attempt {Attempt})",
                            area, providerName, attempt + 1);

                        var context = $"Area: {area}. Provide detailed, actionable testing advice. Focus on practical solutions.";

                        var result = await llmService.GenerateTestCodeAsync(prompt, context);

                        return new KeyValuePair<string, LLMResult>(
                            area,
                            new LLMResult
                            {
                                Content = result,
                                Provider = providerName,
                                ProcessedAt = DateTime.UtcNow,
                                ProcessingTime = DateTime.UtcNow - startTime,
                                TokenCount = EstimateTokenCount(result),
                                Success = true
                            }
                        );
                    }
                    catch (Exception ex)
                    {
                        attempt++;
                        _logger.LogWarning(ex, "Failed to process {Area} with {Provider} (Attempt {Attempt}/{MaxRetries})",
                            area, providerName, attempt, maxRetries);

                        if (attempt > maxRetries)
                        {
                            // Try fallback provider
                            return await TryFallbackProcessingAsync(area, prompt, providerName);
                        }

                        // Wait before retry (exponential backoff)
                        await Task.Delay(1000 * (int)Math.Pow(2, attempt));
                    }
                }

                // This should not be reached
                return new KeyValuePair<string, LLMResult>(
                    area,
                    new LLMResult
                    {
                        Content = "Failed to generate content after multiple attempts",
                        Provider = providerName,
                        ProcessedAt = DateTime.UtcNow,
                        ProcessingTime = DateTime.UtcNow - startTime,
                        Success = false,
                        ErrorMessage = "Max retries exceeded"
                    }
                );
            }

            private async Task<KeyValuePair<string, LLMResult>> TryFallbackProcessingAsync(
                string area, string prompt, string failedProvider)
            {
                var fallbackOrder = _strategyOptions.FallbackOrder ??
                    new[] { "chatgpt", "claude", "deepseek", "gemini", "llama" };

                foreach (var fallbackProvider in fallbackOrder)
                {
                    if (fallbackProvider == failedProvider)
                        continue;

                    try
                    {
                        _logger.LogInformation("Attempting fallback to {Provider} for {Area}",
                            fallbackProvider, area);

                        var llmService = GetLLMService(fallbackProvider);
                        var context = $"Area: {area}. Provide detailed, actionable testing advice. Fallback from {failedProvider}.";

                        var result = await llmService.GenerateTestCodeAsync(prompt, context);

                        return new KeyValuePair<string, LLMResult>(
                            area,
                            new LLMResult
                            {
                                Content = result,
                                Provider = fallbackProvider,
                                ProcessedAt = DateTime.UtcNow,
                                ProcessingTime = TimeSpan.Zero,
                                Success = true,
                                TokenCount = EstimateTokenCount(result)
                            }
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Fallback to {Provider} also failed", fallbackProvider);
                    }
                }

                throw new LLMCoordinationException($"All LLM providers failed for area {area}");
            }

            private ILLMService GetLLMService(string providerName)
            {
                // Get all registered LLM services
                var services = _serviceProvider.GetServices<ILLMService>();
                var service = services.FirstOrDefault(s => s.CanHandleProvider(providerName));

                return service ?? throw new LLMCoordinationException($"No LLM service found for provider: {providerName}");
            }

            private int EstimateTokenCount(string text)
            {
                // Rough estimate: 1 token ≈ 4 characters
                return text?.Length / 4 ?? 0;
            }
        }
    }
