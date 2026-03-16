using Introduction.Exceptions;
using Introduction.Models;
using Introduction.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;


namespace Introduction.Controllers
{
    // Controllers/TestGenController.cs
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TestGenController : ControllerBase
    {
        private readonly ITestGenerationService _testGenService;
        private readonly ILogger<TestGenController> _logger;

        public TestGenController(
            ITestGenerationService testGenService,
            ILogger<TestGenController> logger)
        {
            _testGenService = testGenService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateTestCase([FromBody]
        TestCaseRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                _logger.LogWarning("Invalid request: {@Errors}",
                validationErrors);
                return BadRequest(new AIError
                {
                    Message = "Hmm, there's an issue with your request.",
                    Suggestion = string.Join(" ", validationErrors),
                    Recoverable = true,
                    Provider = "validation"
                });
            }

            try
            {
                _logger.LogInformation(@"Generating test for prompt: {PromptLength} chars using { Provider}
                ", 
                     request.Prompt.Length, request.LlmProvider);
                var result = await
                            _testGenService.GenerateTestAsync(request);
                return Ok(result);
            }
            catch (AIProviderException ex)
            {
                _logger.LogError(ex, "AI provider {Provider} failed",
                request.LlmProvider);

                return StatusCode(503, new AIError
                {
                    Message = $"The {request.LlmProvider} service is having a moment.",
                    Suggestion = "Try again in a bit, or switch to a different provider.",
                    Recoverable = true,
                    Provider = request.LlmProvider
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error generating test");
                return StatusCode(500, new AIError
            {
                Message = "Something unexpected went wrong on our  end.",
                Suggestion = "Contact support if this keeps happening.",
                Recoverable = false,
                Provider = request.LlmProvider
            });
            }
        }
}

// Services/Interfaces/ILLMService.cs
public interface ILLMService
    {
        Task<string> GenerateTestCodeAsync(string prompt, string
                                           context);
        bool CanHandleProvider(string providerName);
        string ProviderName { get; }
    }

    // Services/Implementations/ChatGPTService.cs
    public class ChatGPTService : ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<ChatGPTService> _logger;

        public ChatGPTService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<ChatGPTService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public string ProviderName => "chatgpt";

        public bool CanHandleProvider(string providerName) =>
            providerName.Equals("chatgpt",
                                 StringComparison.OrdinalIgnoreCase);

        public async Task<string> GenerateTestCodeAsync(string prompt,
        string context)
        {
            var apiKey = _config["LLMProviders:ChatGPT:ApiKey"];
            var endpoint = _config["LLMProviders:ChatGPT:Endpoint"];

            var chatRequest = new
            {
                model = "gpt-4",
                messages = new[]
                {
                new { role = "system", content = "You are a senior test automation engineer. Generate clean, maintainable test code." },
                new { role = "user", content =
                      $"{context}\n\n{prompt}" }
            },
            temperature = 0.2 // Low temperature for consistent, deterministic test code
        };

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsJsonAsync(endpoint,
                                 chatRequest);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("ChatGPT API failed with status { StatusCode}", response.StatusCode);
                throw new AIProviderException($"ChatGPT service unavailable: { response.StatusCode }");
            }

            var result = await
                response.Content.ReadFromJsonAsync<ChatGPTResponse>();
            return
             result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim()
                   ?? "// Sorry, the AI returned an empty response. Try again ? ";
        }
    }

    // Services/Implementations/TestGenerationService.cs
    public class TestGenerationService : ITestGenerationService
    {
        private readonly IEnumerable<ILLMService> _llmServices;
        private readonly ICodeTransformer _codeTransformer;

        public TestGenerationService(
            IEnumerable<ILLMService> llmServices,
            ICodeTransformer codeTransformer)
        {
            _llmServices = llmServices;
            _codeTransformer = codeTransformer;
        }

        public async Task<TestCaseResponse>
        GenerateTestAsync(TestCaseRequest request)
        {
            var provider = _llmServices.FirstOrDefault(s =>
                           s.CanHandleProvider(request.LlmProvider))
                           ?? throw new AIProviderException($"No service found for provider: {request.LlmProvider}");

            var rawAICode = await
                   provider.GenerateTestCodeAsync(request.Prompt,
                   request.Context ?? string.Empty);

            // This is where the magic happens—turning AI text into real scripts
            var transformedCode = await
            _codeTransformer.TransformToExecutableScriptAsync(
                rawAICode,
                request.Prompt
            );

            return new TestCaseResponse
            {
                GeneratedCode = transformedCode,
                TestFramework = transformedCode.Contains("page.goto") ?
                                "playwright" : "selenium",
                Explanation = new[] { "Generated by AI, reviewed by you.", "Check selectors match your actual page." },
                EstimatedComplexity =
                                EstimateComplexity(transformedCode),
                PotentialFlakyPoints =
                               FindFlakyPatterns(transformedCode),
                ChosenProvider = provider.ProviderName
            };



        }


        private int EstimateComplexity(string code) => code.Split('\n').Length / 10;

        private string[] FindFlakyPatterns(string code)
        {
            var warnings = new List<string>();
            if (code.Contains("Thread.Sleep") ||
                code.Contains("Task.Delay"))
                warnings.Add("Fixed delays can cause flakiness—consider explicit waits.");
            if (code.Contains("By.XPath(") && code.Contains("//div/"))
                warnings.Add("Complex XPaths may break with UI changes.");
            return warnings.ToArray();
        }
    }
    
 }

    


