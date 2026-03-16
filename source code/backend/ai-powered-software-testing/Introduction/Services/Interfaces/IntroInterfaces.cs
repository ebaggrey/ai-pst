using Introduction.Models;

namespace Introduction.Services.Interfaces
{
    public interface ITestGenerationService
    {
        Task<TestCaseResponse> GenerateTestAsync(TestCaseRequest request);
    }


    public interface ICodeTransformer
    {
        Task<string> TransformToExecutableScriptAsync(string rawAICode, string originalPrompt);
    }
}
