namespace Introduction.Services
{
    using global::Introduction.Services.Interfaces;
    using Microsoft.Extensions.Logging;

    namespace Introduction.Services.Implementations
    {
        public class CodeTransformerService : ICodeTransformer
        {
            private readonly ILogger<CodeTransformerService> _logger;

            public CodeTransformerService(ILogger<CodeTransformerService> logger)
            {
                _logger = logger;
            }

            public async Task<string> TransformToExecutableScriptAsync(string rawAICode, string originalPrompt)
            {
                // In a real implementation, you'd parse the AI response
                // and convert it to executable test scripts
                _logger.LogInformation("Transforming AI code for prompt: {Prompt}",
                    originalPrompt.Substring(0, Math.Min(50, originalPrompt.Length)));

                // For now, just clean up and format the code
                return await Task.Run(() =>
                {
                    // Remove markdown code blocks if present
                    var cleanedCode = rawAICode
                        .Replace("```csharp", "")
                        .Replace("```javascript", "")
                        .Replace("```python", "")
                        .Replace("```typescript", "")
                        .Replace("```", "")
                        .Trim();

                    // Add proper indentation and comments
                    return $"// Generated from: {originalPrompt}\n\n{cleanedCode}";
                });
            }
        }
    }
}
