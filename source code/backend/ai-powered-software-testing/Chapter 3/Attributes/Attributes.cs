namespace Chapter_3.Attributes
{
    using Chapter_3.Models.Domain;
    using System.ComponentModel.DataAnnotations;

    // Custom validation attribute for GeneratedTest
    public class ValidateGeneratedTestAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is GeneratedTest test)
            {
                var errors = new List<string>();

                // 1. Validate content is not empty
                if (string.IsNullOrWhiteSpace(test.Content))
                    errors.Add("Test content cannot be empty");

                // 2. Validate confidence score range
                if (test.ConfidenceScore < 0 || test.ConfidenceScore > 1)
                    errors.Add($"Confidence score must be between 0 and 1 (current: {test.ConfidenceScore})");

                // 3. Validate generated timestamp is not in the future
                if (test.GeneratedAt > DateTime.UtcNow.AddMinutes(5))
                    errors.Add("Generated timestamp cannot be more than 5 minutes in the future");

                // 4. Validate required fields
                if (string.IsNullOrWhiteSpace(test.TestType))
                    errors.Add("Test type is required");

                if (string.IsNullOrWhiteSpace(test.Framework))
                    errors.Add("Test framework is required");

                if (string.IsNullOrWhiteSpace(test.Language))
                    errors.Add("Programming language is required");

                // 5. Validate content length (optional but recommended)
                if (test.Content.Length < 10)
                    errors.Add("Test content is too short (minimum 10 characters)");

                if (test.Content.Length > 100000) // 100KB max
                    errors.Add("Test content is too long (maximum 100,000 characters)");

                // 6. Validate test categories if provided
                if (test.TestCategories != null && test.TestCategories.Any(string.IsNullOrWhiteSpace))
                    errors.Add("Test categories cannot contain empty strings");

                // 7. Validate tags if provided
                if (test.Tags != null && test.Tags.Any(string.IsNullOrWhiteSpace))
                    errors.Add("Tags cannot contain empty strings");

                // 8. Validate confidence breakdown if provided
                if (test.ConfidenceBreakdown != null)
                {
                    foreach (var kvp in test.ConfidenceBreakdown)
                    {
                        if (kvp.Value < 0 || kvp.Value > 1)
                            errors.Add($"Confidence breakdown value for '{kvp.Key}' must be between 0 and 1");
                    }
                }

                // 9. Custom business rule: High confidence tests should have minimal content
                if (test.ConfidenceScore > 0.9 && test.Content.Length < 50)
                {
                    errors.Add("High confidence tests should have substantial content (minimum 50 characters)");
                }

                // 10. Validate test type is from allowed values
                var allowedTestTypes = new[] { "unit", "integration", "functional", "e2e", "performance", "security", "regression" };
                if (!string.IsNullOrWhiteSpace(test.TestType) && !allowedTestTypes.Contains(test.TestType.ToLower()))
                {
                    errors.Add($"Test type '{test.TestType}' is not valid. Allowed values: {string.Join(", ", allowedTestTypes)}");
                }

                if (errors.Any())
                {
                    // Get the property name for better error messages
                    var memberNames = validationContext.MemberName != null
                        ? new[] { validationContext.MemberName }
                        : new[] { "GeneratedContent" };

                    return new ValidationResult($"Generated test validation failed: {string.Join("; ", errors)}", memberNames);
                }
            }
            else if (value != null) // Value is not null but not a GeneratedTest
            {
                return new ValidationResult($"Value must be of type GeneratedTest. Actual type: {value.GetType().Name}",
                    new[] { validationContext.MemberName ?? "GeneratedContent" });
            }
            // If value is null, let RequiredAttribute handle it

            return ValidationResult.Success;
        }

        // Optional: Add a method to provide client-side validation (if using ASP.NET Core with jQuery Validation)
        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field contains invalid test data. Please check the test content and metadata.";
        }
    }

    // Optional: Additional validation attribute for specific test types
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateTestTypeAttribute : ValidationAttribute
    {
        private readonly string[] _allowedTypes;

        public ValidateTestTypeAttribute(params string[] allowedTypes)
        {
            _allowedTypes = allowedTypes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string testType)
            {
                if (!_allowedTypes.Contains(testType.ToLower()))
                {
                    return new ValidationResult(
                        $"Test type '{testType}' is not allowed. Allowed types: {string.Join(", ", _allowedTypes)}",
                        new[] { validationContext.MemberName });
                }
            }

            return ValidationResult.Success;
        }
    }

    // Optional: Composite validation attribute for complex validation scenarios
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class ValidateGeneratedTestComprehensiveAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Use service provider to get additional validation services if needed
            var serviceProvider = validationContext.GetService(typeof(IServiceProvider)) as IServiceProvider;

            if (value is GeneratedTest test)
            {
                var validationResults = new List<ValidationResult>();

                // Validate using data annotations on the GeneratedTest properties
                var context = new ValidationContext(test, serviceProvider, null);
                Validator.TryValidateObject(test, context, validationResults, validateAllProperties: true);

                // Add custom business logic validations
                if (test.ConfidenceScore > 0.8 && test.Content.Split('\n').Length < 3)
                {
                    validationResults.Add(new ValidationResult(
                        "High confidence tests should have multiple lines of code",
                        new[] { nameof(test.Content) }));
                }

                if (test.TestType == "security" && !test.Content.Contains("Assert"))
                {
                    validationResults.Add(new ValidationResult(
                        "Security tests should include assertions",
                        new[] { nameof(test.Content) }));
                }

                if (validationResults.Any())
                {
                    var errorMessages = validationResults.Select(r => r.ErrorMessage);
                    return new ValidationResult($"Generated test validation errors: {string.Join("; ", errorMessages)}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
