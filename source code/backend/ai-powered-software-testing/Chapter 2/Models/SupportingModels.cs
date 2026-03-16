using System.ComponentModel.DataAnnotations;

namespace Chapter_2.Models
{
    public class OnboardingSettings
    {
        public PhaseSettings[] Phases { get; set; } = Array.Empty<PhaseSettings>();
        public Dictionary<string, string> TaskToLLMMapping { get; set; } = new();
        public Dictionary<string, string> Checkpoints { get; set; } = new();
    }

    public class PhaseSettings
    {
        public string Name { get; set; } = string.Empty;
        public string Weeks { get; set; } = string.Empty;
        public string[] Focus { get; set; } = Array.Empty<string>();
        public string[] SuccessMetrics { get; set; } = Array.Empty<string>();
        public string AIAssistanceLevel { get; set; } = "low";
        public string[] ExpectedOutputs { get; set; } = Array.Empty<string>();
    }

    public class First90DaysSettings
    {
        public bool AllowExperimentalFeatures { get; set; } = true;
        public decimal MaximumAICostPerDay { get; set; } = 5.00m;
        public bool RequiredHumanReview { get; set; } = true;
        public string ProgressReportingFrequency { get; set; } = "weekly";
        public string MentorAssignment { get; set; } = "automatic";
        public ResourceLibrary ResourceLibrary { get; set; } = new();
    }

    public class ResourceLibrary
    {
        public string[] Videos { get; set; } = Array.Empty<string>();
        public string[] Articles { get; set; } = Array.Empty<string>();
        public string[] CodeLabs { get; set; } = Array.Empty<string>();
        public string[] Community { get; set; } = Array.Empty<string>();
    }


    public class CodeFileInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; }  // Changed from SizeInBytes to Size
        public DateTime LastModified { get; set; }
        public string Extension { get; set; } = string.Empty;
        public string Directory { get; set; } = string.Empty;
    }

    // And update ProjectInfo to be consistent:
    public class ProjectInfo
    {
        public string Name { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string[] TargetFrameworks { get; set; } = Array.Empty<string>();
        public string[] Dependencies { get; set; } = Array.Empty<string>();
        public string[] ReferencedProjects { get; set; } = Array.Empty<string>();
        public Dictionary<string, string> Properties { get; set; } = new();
        public DateTime LastModified { get; set; }
        public long Size { get; set; }  // Changed from SizeInBytes to Size
        public CodeFileInfo[] SourceFiles { get; set; } = Array.Empty<CodeFileInfo>();
    }

    //public class ProjectInfo
    //{
    //    public string Name { get; set; } = string.Empty;
    //    public string FilePath { get; set; } = string.Empty;
    //    public string Type { get; set; } = string.Empty; // e.g., "csproj", "fsproj", "vbproj"
    //    public string[] TargetFrameworks { get; set; } = Array.Empty<string>();
    //    public string[] Dependencies { get; set; } = Array.Empty<string>();
    //    public string[] ReferencedProjects { get; set; } = Array.Empty<string>();
    //    public Dictionary<string, string> Properties { get; set; } = new();
    //    public DateTime LastModified { get; set; }
    //    public long SizeInBytes { get; set; }
    //    public CodeFileInfo[] SourceFiles { get; set; } = Array.Empty<CodeFileInfo>();
    //}

    // Updated CodeStructure to use CodeFileInfo
    public class CodeStructure
    {
        public string RootPath { get; set; } = string.Empty;
        public CodeFileInfo[] Files { get; set; } = Array.Empty<CodeFileInfo>();
        public string[] Directories { get; set; } = Array.Empty<string>();
        public ProjectInfo[] Projects { get; set; } = Array.Empty<ProjectInfo>();
        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    }






    public class FirstFixRequest
    {
        [Required]
        public string FlakyTestPath { get; set; } = string.Empty;

        [Required]
        [AllowedExtensions(new[] { ".cs", ".js", ".ts", ".java", ".py" })]
        public string TestContent { get; set; } = string.Empty;

        public string[] ObservedFailures { get; set; } = Array.Empty<string>();

        [Range(1, 100)]
        public int FailureRatePercent { get; set; }

        public string EnvironmentContext { get; set; } = "local-and-ci";

        [Required]
        public string FixStrategy { get; set; } = "conservative"; // or "aggressive", "refactor"

        // Dependencies for this model:
        // 1. Required attribute - from System.ComponentModel.DataAnnotations
        // 2. Range attribute - from System.ComponentModel.DataAnnotations
        // 3. AllowedExtensionsAttribute - custom validation attribute
    }

    // Dependency for FirstFixRequest
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string content)
            {
                var hasExtension = _extensions.Any(ext =>
                    validationContext.MemberName?.EndsWith(ext, StringComparison.OrdinalIgnoreCase) == true);

                if (!hasExtension)
                {
                    return new ValidationResult(
                        $"Test file should be one of: {string.Join(", ", _extensions)}");
                }
            }

            return ValidationResult.Success;
        }
    }

    // Additional dependencies that might be needed for FirstFixRequest
    namespace System.ComponentModel.DataAnnotations
    {
        public class RequiredAttribute : Attribute
        {
            public bool AllowEmptyStrings { get; set; }
        }

        public class RangeAttribute : Attribute
        {
            public RangeAttribute(int minimum, int maximum) { }
            public RangeAttribute(double minimum, double maximum) { }
            public RangeAttribute(Type type, string minimum, string maximum) { }
        }

        public abstract class ValidationAttribute : Attribute
        {
            public string ErrorMessage { get; set; } = string.Empty;
            public string ErrorMessageResourceName { get; set; } = string.Empty;
            public Type ErrorMessageResourceType { get; set; }

            protected virtual ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                return ValidationResult.Success;
            }
        }

        public sealed class ValidationResult
        {
            public string ErrorMessage { get; }
            public IEnumerable<string> MemberNames { get; }

            public ValidationResult(string errorMessage) : this(errorMessage, null) { }

            public ValidationResult(string errorMessage, IEnumerable<string> memberNames)
            {
                ErrorMessage = errorMessage;
                MemberNames = memberNames ?? Enumerable.Empty<string>();
            }

            public static readonly ValidationResult Success = null;
        }

        public class ValidationContext
        {
            public ValidationContext(object instance) { }
            public string MemberName { get; set; }
            public object ObjectInstance { get; }
            public string DisplayName { get; set; }
        }
    }




}
