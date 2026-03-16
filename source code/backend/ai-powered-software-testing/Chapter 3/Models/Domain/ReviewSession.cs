using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chapter_3.Models.Domain
{

    // Models/Domain/GeneratedContent.cs - Specific model for ReviewSession
    public class GeneratedContent : ICloneable
    {
        [Required]
        [JsonPropertyName("test")]
        public GeneratedTest Test { get; set; } = new();

        [Required]
        [JsonPropertyName("testType")]
        public string TestType => Test.TestType; // Delegated property

        [JsonPropertyName("sessionContext")]
        public SessionContentContext Context { get; set; } = new();

        [JsonPropertyName("modifications")]
        public ContentModification[] Modifications { get; set; } = Array.Empty<ContentModification>();

        [JsonPropertyName("reviewStatus")]
        public ContentReviewStatus ReviewStatus { get; set; } = new();

        [JsonPropertyName("aiAnalysis")]
        public ContentAiAnalysis AiAnalysis { get; set; } = new();

        [JsonPropertyName("humanFeedback")]
        public HumanFeedback[] HumanFeedback { get; set; } = Array.Empty<HumanFeedback>();

        [JsonPropertyName("version")]
        public int Version { get; set; } = 1;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("metadata")]
        public GeneratedContentMetadata Metadata { get; set; } = new();

        // Convenience property to access test content
        public string Content => Test.Content;

        // Convenience property to access confidence score
        public double ConfidenceScore => Test.ConfidenceScore;

        // Clone implementation
        public object Clone()
        {
            return new GeneratedContent
            {
                Test = (GeneratedTest)Test.Clone(),
                Context = (SessionContentContext)Context?.Clone() ?? new SessionContentContext(),
                Modifications = Modifications.Select(m => (ContentModification)m.Clone()).ToArray(),
                ReviewStatus = (ContentReviewStatus)ReviewStatus?.Clone() ?? new ContentReviewStatus(),
                AiAnalysis = (ContentAiAnalysis)AiAnalysis?.Clone() ?? new ContentAiAnalysis(),
                HumanFeedback = HumanFeedback.Select(h => (HumanFeedback)h.Clone()).ToArray(),
                Version = Version,
                CreatedAt = CreatedAt,
                LastModified = LastModified,
                Metadata = (GeneratedContentMetadata)Metadata?.Clone() ?? new GeneratedContentMetadata()
            };
        }
    }

    // Dependency models for GeneratedContent

    public class SessionContentContext : ICloneable
    {
        [JsonPropertyName("purpose")]
        public string Purpose { get; set; } = string.Empty;

        [JsonPropertyName("requirements")]
        public string[] Requirements { get; set; } = Array.Empty<string>();

        [JsonPropertyName("constraints")]
        public Dictionary<string, string> Constraints { get; set; } = new();

        [JsonPropertyName("dependencies")]
        public string[] Dependencies { get; set; } = Array.Empty<string>();

        [JsonPropertyName("businessRules")]
        public BusinessRule[] BusinessRules { get; set; } = Array.Empty<BusinessRule>();

        [JsonPropertyName("acceptanceCriteria")]
        public string[] AcceptanceCriteria { get; set; } = Array.Empty<string>();

        public object Clone()
        {
            return new SessionContentContext
            {
                Purpose = Purpose,
                Requirements = Requirements.ToArray(),
                Constraints = new Dictionary<string, string>(Constraints),
                Dependencies = Dependencies.ToArray(),
                BusinessRules = BusinessRules.Select(br => (BusinessRule)br.Clone()).ToArray(),
                AcceptanceCriteria = AcceptanceCriteria.ToArray()
            };
        }
    }

    public class ContentModification : ICloneable
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("type")]
        public string Type { get; set; } = "edit"; // edit, suggestion, refactor, optimization

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("appliedBy")]
        public string AppliedBy { get; set; } = string.Empty;

        [JsonPropertyName("appliedAt")]
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("impact")]
        public ModificationImpact Impact { get; set; } = new();

        [JsonPropertyName("before")]
        public string Before { get; set; } = string.Empty;

        [JsonPropertyName("after")]
        public string After { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;

        public object Clone()
        {
            return new ContentModification
            {
                Id = Id,
                Type = Type,
                Description = Description,
                AppliedBy = AppliedBy,
                AppliedAt = AppliedAt,
                Impact = (ModificationImpact)Impact?.Clone() ?? new ModificationImpact(),
                Before = Before,
                After = After,
                Reason = Reason
            };
        }
    }

    public class ModificationImpact : ICloneable
    {
        [JsonPropertyName("level")]
        public string Level { get; set; } = "low"; // low, medium, high

        [JsonPropertyName("affectedLines")]
        public int[] AffectedLines { get; set; } = Array.Empty<int>();

        [JsonPropertyName("qualityChange")]
        public double QualityChange { get; set; } // -1.0 to 1.0

        [JsonPropertyName("complexityChange")]
        public double ComplexityChange { get; set; } // -1.0 to 1.0

        [JsonPropertyName("coverageChange")]
        public double CoverageChange { get; set; } // -1.0 to 1.0

        public object Clone()
        {
            return new ModificationImpact
            {
                Level = Level,
                AffectedLines = AffectedLines.ToArray(),
                QualityChange = QualityChange,
                ComplexityChange = ComplexityChange,
                CoverageChange = CoverageChange
            };
        }
    }

    public class ContentReviewStatus : ICloneable
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = "pending"; // pending, in_review, approved, rejected, needs_revision

        [JsonPropertyName("progress")]
        public double Progress { get; set; } // 0.0 to 1.0

        [JsonPropertyName("reviewedBy")]
        public string[] ReviewedBy { get; set; } = Array.Empty<string>();

        [JsonPropertyName("reviewedAt")]
        public DateTime[] ReviewedAt { get; set; } = Array.Empty<DateTime>();

        [JsonPropertyName("issuesFound")]
        public int IssuesFound { get; set; }

        [JsonPropertyName("suggestionsApplied")]
        public int SuggestionsApplied { get; set; }

        public object Clone()
        {
            return new ContentReviewStatus
            {
                Status = Status,
                Progress = Progress,
                ReviewedBy = ReviewedBy.ToArray(),
                ReviewedAt = ReviewedAt.ToArray(),
                IssuesFound = IssuesFound,
                SuggestionsApplied = SuggestionsApplied
            };
        }
    }

    public class ContentAiAnalysis : ICloneable
    {
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("riskAreas")]
        public string[] RiskAreas { get; set; } = Array.Empty<string>();

        [JsonPropertyName("suggestions")]
        public AiSuggestion[] Suggestions { get; set; } = Array.Empty<AiSuggestion>();

        [JsonPropertyName("qualityMetrics")]
        public QualityMetrics QualityMetrics { get; set; } = new();

        [JsonPropertyName("generatedAt")]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("modelVersion")]
        public string ModelVersion { get; set; } = string.Empty;

        public object Clone()
        {
            return new ContentAiAnalysis
            {
                Confidence = Confidence,
                RiskAreas = RiskAreas.ToArray(),
                Suggestions = Suggestions.Select(s =>(AiSuggestion)s.Clone()).ToArray(),
                QualityMetrics = (QualityMetrics)QualityMetrics?.Clone() ?? new QualityMetrics(),
                GeneratedAt = GeneratedAt,
                ModelVersion = ModelVersion
            };
        }
    }

    public class HumanFeedback : ICloneable
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("type")]
        public string Type { get; set; } = "comment"; // comment, suggestion, issue, approval

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("providedBy")]
        public string ProvidedBy { get; set; } = string.Empty;

        [JsonPropertyName("providedAt")]
        public DateTime ProvidedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("priority")]
        public string Priority { get; set; } = "medium"; // low, medium, high, critical

        [JsonPropertyName("status")]
        public string Status { get; set; } = "open"; // open, addressed, resolved, dismissed

        [JsonPropertyName("lineNumbers")]
        public int[] LineNumbers { get; set; } = Array.Empty<int>();

        [JsonPropertyName("category")]
        public string Category { get; set; } = "general"; // logic, syntax, style, performance, security

        public object Clone()
        {
            return new HumanFeedback
            {
                Id = Id,
                Type = Type,
                Content = Content,
                ProvidedBy = ProvidedBy,
                ProvidedAt = ProvidedAt,
                Priority = Priority,
                Status = Status,
                LineNumbers = LineNumbers.ToArray(),
                Category = Category
            };
        }
    }

    public class GeneratedContentMetadata : ICloneable
    {
        [JsonPropertyName("source")]
        public string Source { get; set; } = "ai_generated"; // ai_generated, human_written, hybrid

        [JsonPropertyName("generationMethod")]
        public string GenerationMethod { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; } = Array.Empty<string>();

        [JsonPropertyName("customProperties")]
        public Dictionary<string, object> CustomProperties { get; set; } = new();

        [JsonPropertyName("qualityScore")]
        public double QualityScore { get; set; }

        [JsonPropertyName("complexityScore")]
        public double ComplexityScore { get; set; }

        [JsonPropertyName("maintainabilityScore")]
        public double MaintainabilityScore { get; set; }

        public object Clone()
        {
            return new GeneratedContentMetadata
            {
                Source = Source,
                GenerationMethod = GenerationMethod,
                Tags = Tags.ToArray(),
                CustomProperties = new Dictionary<string, object>(CustomProperties),
                QualityScore = QualityScore,
                ComplexityScore = ComplexityScore,
                MaintainabilityScore = MaintainabilityScore
            };
        }
    }

    // Additional dependency models

    public class BusinessRule : ICloneable
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("condition")]
        public string Condition { get; set; } = string.Empty;

        [JsonPropertyName("priority")]
        public string Priority { get; set; } = "medium";

        public object Clone()
        {
            return new BusinessRule
            {
                Id = Id,
                Description = Description,
                Condition = Condition,
                Priority = Priority
            };
        }
    }

    public class QualityMetrics : ICloneable
    {
        [JsonPropertyName("cyclomaticComplexity")]
        public int CyclomaticComplexity { get; set; }

        [JsonPropertyName("linesOfCode")]
        public int LinesOfCode { get; set; }

        [JsonPropertyName("maintainabilityIndex")]
        public double MaintainabilityIndex { get; set; }

        [JsonPropertyName("testCoverage")]
        public double TestCoverage { get; set; }

        [JsonPropertyName("codeDuplication")]
        public double CodeDuplication { get; set; }

        [JsonPropertyName("securityIssues")]
        public int SecurityIssues { get; set; }

        public object Clone()
        {
            return new QualityMetrics
            {
                CyclomaticComplexity = CyclomaticComplexity,
                LinesOfCode = LinesOfCode,
                MaintainabilityIndex = MaintainabilityIndex,
                TestCoverage = TestCoverage,
                CodeDuplication = CodeDuplication,
                SecurityIssues = SecurityIssues
            };
        }
    }





    // Models/Domain/GeneratedTest.cs -
    public class GeneratedTest : ICloneable
    {
        [Required]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Range(0, 1)]
        [JsonPropertyName("confidenceScore")]
        public double ConfidenceScore { get; set; }

        [Required]
        [JsonPropertyName("generatedAt")]
        public DateTime GeneratedAt { get; set; }

        [Required]
        [JsonPropertyName("testType")]
        public string TestType { get; set; } = "unit"; // unit, integration, functional, e2e, performance, security, etc.

        [Required]
        [JsonPropertyName("framework")]
        public string Framework { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public string Language { get; set; } = "C#";

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; } = Array.Empty<string>();

        [JsonPropertyName("dependencies")]
        public string[] Dependencies { get; set; } = Array.Empty<string>();

        [JsonPropertyName("estimatedExecutionTimeMs")]
        public int EstimatedExecutionTimeMs { get; set; }

        [JsonPropertyName("complexity")]
        public string Complexity { get; set; } = "medium"; // simple, medium, complex

        [JsonPropertyName("testCategories")]
        public string[] TestCategories { get; set; } = Array.Empty<string>();

        [JsonPropertyName("modelVersion")]
        public string ModelVersion { get; set; } = string.Empty;

        [JsonPropertyName("generationId")]
        public string GenerationId { get; set; } = string.Empty;

        [JsonPropertyName("confidenceBreakdown")]
        public Dictionary<string, double> ConfidenceBreakdown { get; set; } = new();

        [JsonPropertyName("metadata")]
        public GeneratedTestMetadata Metadata { get; set; } = new();

        [JsonPropertyName("assertions")]
        public TestAssertion[] Assertions { get; set; } = Array.Empty<TestAssertion>();

        [JsonPropertyName("setup")]
        public string Setup { get; set; } = string.Empty;

        [JsonPropertyName("teardown")]
        public string Teardown { get; set; } = string.Empty;

        [JsonPropertyName("testCases")]
        public TestCase[] TestCases { get; set; } = Array.Empty<TestCase>();

        // ICloneable implementation for deep cloning
        public object Clone()
        {
            return new GeneratedTest
            {
                Content = this.Content,
                ConfidenceScore = this.ConfidenceScore,
                GeneratedAt = this.GeneratedAt,
                TestType = this.TestType,
                Framework = this.Framework,
                Language = this.Language,
                Tags = this.Tags.ToArray(),
                Dependencies = this.Dependencies.ToArray(),
                EstimatedExecutionTimeMs = this.EstimatedExecutionTimeMs,
                Complexity = this.Complexity,
                TestCategories = this.TestCategories.ToArray(),
                ModelVersion = this.ModelVersion,
                GenerationId = this.GenerationId,
                ConfidenceBreakdown = new Dictionary<string, double>(this.ConfidenceBreakdown),
                Metadata = (GeneratedTestMetadata)this.Metadata.Clone() ?? new GeneratedTestMetadata(),
                Assertions = this.Assertions.Select(a =>(TestAssertion)a.Clone()).ToArray(),
                Setup = this.Setup,
                Teardown = this.Teardown,
                TestCases = this.TestCases.Select(tc => (TestCase)tc.Clone()).ToArray()
            };
        }
    }

    // Dependency model for GeneratedTest metadata
    public class GeneratedTestMetadata : ICloneable
    {
        [JsonPropertyName("sourceFile")]
        public string SourceFile { get; set; } = string.Empty;

        [JsonPropertyName("targetMethod")]
        public string TargetMethod { get; set; } = string.Empty;

        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; } = new();

        [JsonPropertyName("coverageAreas")]
        public string[] CoverageAreas { get; set; } = Array.Empty<string>();

        [JsonPropertyName("qualityScore")]
        public double QualityScore { get; set; }

        [JsonPropertyName("isFlaky")]
        public bool IsFlaky { get; set; }

        [JsonPropertyName("requiresSetup")]
        public bool RequiresSetup { get; set; }

        [JsonPropertyName("hasSideEffects")]
        public bool HasSideEffects { get; set; }

        [JsonPropertyName("generationContext")]
        public Dictionary<string, object> GenerationContext { get; set; } = new();

        [JsonPropertyName("customProperties")]
        public Dictionary<string, object> CustomProperties { get; set; } = new();

        public object Clone()
        {
            return new GeneratedTestMetadata
            {
                SourceFile = this.SourceFile,
                TargetMethod = this.TargetMethod,
                Parameters = new Dictionary<string, string>(this.Parameters),
                CoverageAreas = this.CoverageAreas.ToArray(),
                QualityScore = this.QualityScore,
                IsFlaky = this.IsFlaky,
                RequiresSetup = this.RequiresSetup,
                HasSideEffects = this.HasSideEffects,
                GenerationContext = new Dictionary<string, object>(this.GenerationContext),
                CustomProperties = new Dictionary<string, object>(this.CustomProperties)
            };
        }
    }

    // Dependency model for test assertions
    public class TestAssertion : ICloneable
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "equality"; // equality, inequality, null, notnull, exception, etc.

        [JsonPropertyName("expected")]
        public string Expected { get; set; } = string.Empty;

        [JsonPropertyName("actual")]
        public string Actual { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("lineNumber")]
        public int LineNumber { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; } = 1.0;

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();

        public object Clone()
        {
            return new TestAssertion
            {
                Type = this.Type,
                Expected = this.Expected,
                Actual = this.Actual,
                Message = this.Message,
                LineNumber = this.LineNumber,
                Confidence = this.Confidence,
                Metadata = new Dictionary<string, object>(this.Metadata)
            };
        }
    }

    // Dependency model for test cases
    public class TestCase : ICloneable
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("input")]
        public Dictionary<string, object> Input { get; set; } = new();

        [JsonPropertyName("expectedOutput")]
        public object ExpectedOutput { get; set; }

        [JsonPropertyName("setup")]
        public string Setup { get; set; } = string.Empty;

        [JsonPropertyName("teardown")]
        public string Teardown { get; set; } = string.Empty;

        [JsonPropertyName("assertions")]
        public TestAssertion[] Assertions { get; set; } = Array.Empty<TestAssertion>();

        [JsonPropertyName("isDataDriven")]
        public bool IsDataDriven { get; set; }

        [JsonPropertyName("data")]
        public object[] Data { get; set; } = Array.Empty<object>();

        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new();

        public object Clone()
        {
            return new TestCase
            {
                Name = this.Name,
                Description = this.Description,
                Input = new Dictionary<string, object>(this.Input),
                ExpectedOutput = this.ExpectedOutput,
                Setup = this.Setup,
                Teardown = this.Teardown,
                Assertions = this.Assertions.Select(a => (TestAssertion)a.Clone()).ToArray(),
                IsDataDriven = this.IsDataDriven,
                Data = this.Data.ToArray(),
                Metadata = new Dictionary<string, object>(this.Metadata)
            };
        }
    }

    // Extension method for deep cloning (alternative to ICloneable)
    public static class GeneratedTestExtensions
    {
        public static GeneratedTest DeepClone(this GeneratedTest original)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            // Use JSON serialization for a true deep clone
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };

            var json = JsonSerializer.Serialize(original, options);
            return JsonSerializer.Deserialize<GeneratedTest>(json, options)
                ?? throw new InvalidOperationException("Failed to clone GeneratedTest");
        }
    }

    // Helper class for test type validation
    public static class TestTypeValidator
    {
        public static readonly string[] AllowedTestTypes =
        {
        "unit",
        "integration",
        "functional",
        "e2e",
        "performance",
        "load",
        "stress",
        "security",
        "regression",
        "smoke",
        "sanity",
        "accessibility",
        "usability",
        "api",
        "ui",
        "mobile",
        "database",
        "migration"
    };

        public static bool IsValidTestType(string testType)
        {
            return AllowedTestTypes.Contains(testType?.ToLower() ?? string.Empty);
        }

        public static string GetAllowedTestTypesString()
        {
            return string.Join(", ", AllowedTestTypes);
        }
    }
}
