
// Services/LLMService.cs
using Chapter_10.Interfaces;
using Chapter_10.Interfaces.LLM;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;
using System.Text;
using System.Text.Json;

namespace Chapter_10.Services
{
    public class LLMService : ILLMService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LLMService> _logger;
        private readonly IConfiguration _configuration;

        public LLMService(
            IHttpClientFactory httpClientFactory,
            ILogger<LLMService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> GenerateInsightAsync(string prompt, InsightContext context)
        {
            try
            {
                _logger.LogInformation("Generating insight with LLM");

                var httpClient = _httpClientFactory.CreateClient("LLMService");
                var request = new
                {
                    prompt = BuildInsightPrompt(prompt, context),
                    max_tokens = 500,
                    temperature = 0.7
                };

                var response = await httpClient.PostAsJsonAsync("/v1/completions", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<LLMResponse>();
                return result?.Choices?[0]?.Text?.Trim() ?? "Unable to generate insight";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating insight with LLM");
                return FallbackInsightGeneration(prompt, context);
            }
        }

        public async Task<string[]> GenerateRecommendationsAsync(string[] dataPoints, string objective)
        {
            try
            {
                _logger.LogInformation("Generating recommendations with LLM");

                var httpClient = _httpClientFactory.CreateClient("LLMService");
                var request = new
                {
                    prompt = BuildRecommendationPrompt(dataPoints, objective),
                    max_tokens = 300,
                    temperature = 0.8
                };

                var response = await httpClient.PostAsJsonAsync("/v1/completions", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<LLMResponse>();
                var text = result?.Choices?[0]?.Text?.Trim() ?? "";

                return ParseRecommendations(text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating recommendations with LLM");
                return FallbackRecommendations(dataPoints, objective);
            }
        }

        public async Task<DetectedPattern[]> AnalyzePatternsWithLLMAsync(HistoricalTrend[] trends)
        {
            try
            {
                _logger.LogInformation("Analyzing patterns with LLM");

                var httpClient = _httpClientFactory.CreateClient("LLMService");
                var request = new
                {
                    prompt = BuildPatternAnalysisPrompt(trends),
                    max_tokens = 400,
                    temperature = 0.3
                };

                var response = await httpClient.PostAsJsonAsync("/v1/completions", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<LLMResponse>();
                var text = result?.Choices?[0]?.Text?.Trim() ?? "";

                return ParsePatterns(text, trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing patterns with LLM");
                return Array.Empty<DetectedPattern>();
            }
        }

        public async Task<string> InterpretMetricsAsync(MetricValue[] metrics, string[] objectives)
        {
            try
            {
                _logger.LogInformation("Interpreting metrics with LLM");

                var httpClient = _httpClientFactory.CreateClient("LLMService");
                var request = new
                {
                    prompt = BuildInterpretationPrompt(metrics, objectives),
                    max_tokens = 600,
                    temperature = 0.5
                };

                var response = await httpClient.PostAsJsonAsync("/v1/completions", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<LLMResponse>();
                return result?.Choices?[0]?.Text?.Trim() ?? "Unable to interpret metrics";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interpreting metrics with LLM");
                return FallbackInterpretation(metrics, objectives);
            }
        }

        public async Task<OptimizationSuggestion[]> SuggestOptimizationsAsync(
            MetricDefinition[] metrics,
            ResourceConstraint[] constraints)
        {
            try
            {
                _logger.LogInformation("Suggesting optimizations with LLM");

                var httpClient = _httpClientFactory.CreateClient("LLMService");
                var request = new
                {
                    prompt = BuildOptimizationPrompt(metrics, constraints),
                    max_tokens = 500,
                    temperature = 0.6
                };

                var response = await httpClient.PostAsJsonAsync("/v1/completions", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<LLMResponse>();
                var text = result?.Choices?[0]?.Text?.Trim() ?? "";

                return ParseOptimizationSuggestions(text, metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suggesting optimizations with LLM");
                return FallbackOptimizations(metrics, constraints);
            }
        }

        public async Task<bool> ValidateMetricDesignAsync(DesignedMetric[] metrics, string[] principles)
        {
            try
            {
                _logger.LogInformation("Validating metric design with LLM");

                var httpClient = _httpClientFactory.CreateClient("LLMService");
                var request = new
                {
                    prompt = BuildValidationPrompt(metrics, principles),
                    max_tokens = 200,
                    temperature = 0.2
                };

                var response = await httpClient.PostAsJsonAsync("/v1/completions", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<LLMResponse>();
                var text = result?.Choices?[0]?.Text?.Trim().ToLower() ?? "";

                return text.Contains("valid") || text.Contains("yes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating metric design with LLM");
                return true; // Default to true on error
            }
        }

        public async Task<string> GenerateNaturalLanguageExplanationAsync(string metricName, double value, string context)
        {
            try
            {
                _logger.LogInformation("Generating explanation with LLM");

                var httpClient = _httpClientFactory.CreateClient("LLMService");
                var request = new
                {
                    prompt = $"Explain what {metricName} with value {value:F2} means in the context of {context}. Provide a clear, actionable explanation.",
                    max_tokens = 200,
                    temperature = 0.4
                };

                var response = await httpClient.PostAsJsonAsync("/v1/completions", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<LLMResponse>();
                return result?.Choices?[0]?.Text?.Trim() ?? $"{metricName} is at {value:F2}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating explanation with LLM");
                return $"{metricName}: {value:F2}";
            }
        }

        #region Private Helper Methods

        private string BuildInsightPrompt(string prompt, InsightContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Generate actionable insights based on the following metrics and context:");
            sb.AppendLine($"Prompt: {prompt}");

            if (context?.Stakeholders != null)
                sb.AppendLine($"Stakeholders: {string.Join(", ", context.Stakeholders)}");

            if (context?.BusinessGoals != null)
                sb.AppendLine($"Business Goals: {string.Join(", ", context.BusinessGoals)}");

            sb.AppendLine("Provide specific, actionable recommendations with clear next steps.");

            return sb.ToString();
        }

        private string BuildRecommendationPrompt(string[] dataPoints, string objective)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Based on the following data points, generate recommendations to achieve: {objective}");
            sb.AppendLine("Data Points:");
            foreach (var point in dataPoints)
            {
                sb.AppendLine($"- {point}");
            }
            sb.AppendLine("Provide 3-5 specific, prioritized recommendations.");

            return sb.ToString();
        }

        private string BuildPatternAnalysisPrompt(HistoricalTrend[] trends)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Analyze the following historical trends and identify patterns, seasonality, and anomalies:");

            foreach (var trend in trends.Take(10)) // Limit to prevent token overflow
            {
                sb.AppendLine($"Date: {trend.Date:d}, Values: {JsonSerializer.Serialize(trend.MetricValues)}");
            }

            sb.AppendLine("Identify: 1) Overall trends 2) Seasonal patterns 3) Anomalies 4) Correlations");

            return sb.ToString();
        }

        private string BuildInterpretationPrompt(MetricValue[] metrics, string[] objectives)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Interpret the following metrics in the context of these business objectives:");
            sb.AppendLine($"Objectives: {string.Join(", ", objectives)}");
            sb.AppendLine("Metrics:");

            foreach (var metric in metrics)
            {
                sb.AppendLine($"- {metric.MetricName}: {metric.Value} (Target: {metric.Attributes?.GetValueOrDefault("target") ?? "N/A"})");
            }

            sb.AppendLine("Provide a comprehensive interpretation and business impact analysis.");

            return sb.ToString();
        }

        private string BuildOptimizationPrompt(MetricDefinition[] metrics, ResourceConstraint[] constraints)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Suggest optimizations for the following metrics given these constraints:");
            sb.AppendLine("Metrics (MetricId, Name, Cost, Value):");

            foreach (var metric in metrics)
            {
                sb.AppendLine($"- {metric.MetricId}: {metric.Name}, Cost: {metric.CollectionCost}, Value: {metric.BusinessValue}");
            }

            sb.AppendLine("Constraints:");
            foreach (var constraint in constraints)
            {
                sb.AppendLine($"- {constraint.ResourceType}: Max {constraint.MaxAllocation} {constraint.Unit} per {constraint.Period}");
            }

            sb.AppendLine("Suggest which metrics to keep, modify, remove, or add with rationale.");

            return sb.ToString();
        }

        private string BuildValidationPrompt(DesignedMetric[] metrics, string[] principles)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Validate if these metrics align with the design principles:");
            sb.AppendLine($"Principles: {string.Join(", ", principles)}");
            sb.AppendLine("Metrics:");

            foreach (var metric in metrics)
            {
                sb.AppendLine($"- {metric.Name}: {metric.Description}, Target: {metric.TargetValue} {metric.Unit}");
            }

            sb.AppendLine("Respond with 'valid' if they align, otherwise explain why not.");

            return sb.ToString();
        }

        private string[] ParseRecommendations(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<string>();

            return text.Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim('-', ' ', '\t'))
                .Take(5)
                .ToArray();
        }

        private DetectedPattern[] ParsePatterns(string text, HistoricalTrend[] trends)
        {
            var patterns = new List<DetectedPattern>();

            if (string.IsNullOrEmpty(text))
                return patterns.ToArray();

            // Simple parsing logic - in production, use more sophisticated parsing
            if (text.Contains("upward", StringComparison.OrdinalIgnoreCase))
            {
                patterns.Add(new DetectedPattern
                {
                    Type = "trend",
                    Strength = 0.8,
                    Parameters = new Dictionary<string, object> { ["direction"] = "upward" }
                });
            }

            if (text.Contains("seasonal", StringComparison.OrdinalIgnoreCase))
            {
                patterns.Add(new DetectedPattern
                {
                    Type = "seasonality",
                    Strength = 0.7,
                    Parameters = new Dictionary<string, object> { ["period"] = "weekly" }
                });
            }

            return patterns.ToArray();
        }

        private OptimizationSuggestion[] ParseOptimizationSuggestions(string text, MetricDefinition[] metrics)
        {
            var suggestions = new List<OptimizationSuggestion>();

            if (string.IsNullOrEmpty(text) || metrics.Length == 0)
                return suggestions.ToArray();

            foreach (var metric in metrics)
            {
                if (text.Contains($"remove {metric.Name}", StringComparison.OrdinalIgnoreCase))
                {
                    suggestions.Add(new OptimizationSuggestion
                    {
                        MetricId = metric.MetricId,
                        Suggestion = "remove",
                        Confidence = 0.8,
                        Rationale = "LLM analysis suggests low value-to-cost ratio"
                    });
                }
                else if (text.Contains($"consolidate {metric.Name}", StringComparison.OrdinalIgnoreCase))
                {
                    suggestions.Add(new OptimizationSuggestion
                    {
                        MetricId = metric.MetricId,
                        Suggestion = "consolidate",
                        Confidence = 0.7,
                        Rationale = "Potential overlap with other metrics"
                    });
                }
            }

            return suggestions.ToArray();
        }

        private string FallbackInsightGeneration(string prompt, InsightContext context)
        {
            return $"Based on the analysis, we recommend focusing on key metrics that align with business goals. " +
                   $"Consider reviewing {prompt} in the context of {context?.PriorityLevel ?? "current"} priorities.";
        }

        private string[] FallbackRecommendations(string[] dataPoints, string objective)
        {
            return new[]
            {
                $"Focus on improving metrics related to {objective}",
                "Establish baseline measurements",
                "Review and adjust targets quarterly",
                "Implement automated data collection",
                "Share insights with stakeholders"
            };
        }

        private string FallbackInterpretation(MetricValue[] metrics, string[] objectives)
        {
            var avgValue = metrics.Average(m => m.Value);
            return $"Overall metric performance is at {avgValue:F1}% of target. " +
                   $"Most aligned with objective: {objectives.FirstOrDefault() ?? "quality improvement"}.";
        }

        private OptimizationSuggestion[] FallbackOptimizations(MetricDefinition[] metrics, ResourceConstraint[] constraints)
        {
            return metrics.Select(m => new OptimizationSuggestion
            {
                MetricId = m.MetricId,
                Suggestion = m.CollectionCost > 50 ? "review" : "keep",
                Confidence = 0.6,
                Rationale = "Based on cost analysis"
            }).ToArray();
        }

        #endregion
    }

    // LLM API Response Models
    public class LLMResponse
    {
        public LLMChoice[]? Choices { get; set; }
    }

    public class LLMChoice
    {
        public string? Text { get; set; }
    }
}
