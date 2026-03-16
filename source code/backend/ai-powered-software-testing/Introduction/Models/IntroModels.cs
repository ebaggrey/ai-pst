using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Introduction.Models
{
    public class TestCaseRequest
    {
        [Required(ErrorMessage = "Tell us what to test! A prompt is required.")]
        [MinLength(10, ErrorMessage = "Give the AI a bit more to work with—at least 10 characters.")]
        public string Prompt { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(chatgpt|claude|deepseek|gemini|llama)$",
            ErrorMessage = "Pick a valid AI partner: chatgpt, claude, deepseek, gemini, or llama.")]
        public string LlmProvider { get; set; } = "chatgpt";

        public string? Context { get; set; }

        [Range(1, 3, ErrorMessage = "Urgency must be between 1 (thoughtful) and 3 (fast).")]
        public int Urgency { get; set; } = 2; // 1=thoughtful, 2=balanced, 3=fast
    }


    public class TestCaseResponse
    {
        public string GeneratedCode { get; set; } = string.Empty;
        public string TestFramework { get; set; } = "playwright";
        public string[] Explanation { get; set; } = Array.Empty<string>();
        public int EstimatedComplexity { get; set; } // 1-10 scale
        public string[] PotentialFlakyPoints { get; set; } = Array.Empty<string>();
        public string ChosenProvider { get; set; } = string.Empty;
    }


    public class AIError
    {
        public string ErrorId { get; set; } = Guid.NewGuid().ToString();
        public string Message { get; set; } = string.Empty;
        public string Suggestion { get; set; } = string.Empty;
        public bool Recoverable { get; set; } = true;
        public string Provider { get; set; } = string.Empty;
    }


    public class ChatGPTResponse
    {
        [JsonPropertyName("choices")]
        public List<ChatGPTChoice>? Choices { get; set; }
    }

    public class ChatGPTChoice
    {
        [JsonPropertyName("message")]
        public ChatGPTMessage? Message { get; set; }
    }

    public class ChatGPTMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }


}


