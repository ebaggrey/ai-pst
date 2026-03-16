//namespace Chapter2_Ext.Services
//{
//    public class PatternValidator
//    {
//    }
//}


using Chapter2_Ext;
using Chapter2_Ext.Models;

using System.Text.RegularExpressions;


namespace Chapter2_Ext.Services
{
    public class PatternValidator : IPatternValidator
    {
        private readonly ILogger<PatternValidator> _logger;

        // Validation rules configuration
        private readonly ValidationRules _validationRules;

        public PatternValidator(ILogger<PatternValidator> logger)
        {
            _logger = logger;
            _validationRules = new ValidationRules
            {
                MinimumCodeExamples = 2,
                MaximumCodeExamples = 10,
                RequiredConfigKeys = new List<string> { "timeout", "retryCount" },
                ValidPatternNameRegex = @"^[a-z0-9\-]+$",
                MinimumDosAndDonts = 3,
                MaximumDosAndDonts = 10,
                ValidLearningCurves = new List<string> { "easy", "medium", "steep" },
                ValidMaintenanceCosts = new List<string> { "low", "medium", "high" },
                MinimumRepeatabilityScore = 60,
                MaximumTeamSatisfaction = 10,
                RequiredPromptTemplates = 2,
                RequiredValidationRules = 1,
                MinimumCommonPitfalls = 2
            };
        }

        public async Task<bool> ValidatePattern(TestingPatternDto pattern)
        {
            try
            {
                _logger.LogInformation("Validating pattern: {PatternId} - {PatternName}",
                    pattern.Id, pattern.Name);

                var issues = await GetValidationIssues(pattern);
                return !issues.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating pattern: {PatternId}", pattern.Id);
                throw new CustomExceptions.PatternValidationException(
                    "Pattern validation failed due to internal error",
                    new PatternError
                    {
                        PatternArea = pattern.Area,
                        FailureType = "validation",
                        Symptoms = new List<string> { "Validation service error" },
                        RootCause = ex.Message,
                        MitigationSteps = new List<string> { "Retry validation", "Check validator service health" }
                    });
            }
        }

        public async Task<List<string>> GetValidationIssues(TestingPatternDto pattern)
        {
            var issues = new List<string>();

            // Basic validation
            issues.AddRange(ValidateBasicProperties(pattern));

            // Implementation validation
            issues.AddRange(ValidateImplementation(pattern.Implementation));

            // Quality indicators validation
            issues.AddRange(ValidateQualityIndicators(pattern.QualityIndicators));

            // AI assistance validation
            issues.AddRange(ValidateAiAssistance(pattern.AiAssistance));

            // Adoption metrics validation
            issues.AddRange(ValidateAdoptionMetrics(pattern.AdoptionMetrics));

            // Business logic validation
            issues.AddRange(await ValidateBusinessRules(pattern));

            _logger.LogDebug("Found {IssueCount} validation issues for pattern: {PatternId}",
                issues.Count, pattern.Id);

            return issues;
        }

        private List<string> ValidateBasicProperties(TestingPatternDto pattern)
        {
            var issues = new List<string>();

            if (string.IsNullOrWhiteSpace(pattern.Id))
                issues.Add("Pattern ID is required");

            if (string.IsNullOrWhiteSpace(pattern.Name))
                issues.Add("Pattern name is required");
            else if (!Regex.IsMatch(pattern.Name, _validationRules.ValidPatternNameRegex))
                issues.Add($"Pattern name '{pattern.Name}' must contain only lowercase letters, numbers, and hyphens");

            if (string.IsNullOrWhiteSpace(pattern.Area))
                issues.Add("Pattern area is required");

            if (string.IsNullOrWhiteSpace(pattern.ProblemStatement))
                issues.Add("Problem statement is required");
            else if (pattern.ProblemStatement.Length < 20)
                issues.Add("Problem statement must be at least 20 characters");

            if (string.IsNullOrWhiteSpace(pattern.Solution))
                issues.Add("Solution description is required");
            else if (pattern.Solution.Length < 30)
                issues.Add("Solution description must be at least 30 characters");

            if (!new List<string> { "draft", "active", "deprecated", "archived" }.Contains(pattern.Status))
                issues.Add($"Invalid status: {pattern.Status}. Must be one of: draft, active, deprecated, archived");

            return issues;
        }

        private List<string> ValidateImplementation(PatternImplementation implementation)
        {
            var issues = new List<string>();

            if (implementation == null)
            {
                issues.Add("Implementation details are required");
                return issues;
            }

            // Validate code examples
            if (implementation.CodeExamples == null || !implementation.CodeExamples.Any())
                issues.Add("At least one code example is required");
            else if (implementation.CodeExamples.Count < _validationRules.MinimumCodeExamples)
                issues.Add($"At least {_validationRules.MinimumCodeExamples} code examples are required");
            else if (implementation.CodeExamples.Count > _validationRules.MaximumCodeExamples)
                issues.Add($"Maximum {_validationRules.MaximumCodeExamples} code examples allowed");

            if (implementation.CodeExamples != null)
            {
                for (int i = 0; i < implementation.CodeExamples.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(implementation.CodeExamples[i]))
                        issues.Add($"Code example {i + 1} cannot be empty");
                    else if (implementation.CodeExamples[i].Length < 10)
                        issues.Add($"Code example {i + 1} must be at least 10 characters");
                }
            }

            // Validate configuration
            if (implementation.Configuration != null)
            {
                foreach (var requiredKey in _validationRules.RequiredConfigKeys)
                {
                    if (!implementation.Configuration.ContainsKey(requiredKey))
                        issues.Add($"Configuration must include '{requiredKey}' key");
                }

                // Validate specific configuration values
                if (implementation.Configuration.ContainsKey("timeout"))
                {
                    if (!int.TryParse(implementation.Configuration["timeout"]?.ToString(), out int timeout) || timeout <= 0)
                        issues.Add("Timeout configuration must be a positive integer");
                }

                if (implementation.Configuration.ContainsKey("retryCount"))
                {
                    if (!int.TryParse(implementation.Configuration["retryCount"]?.ToString(), out int retryCount) || retryCount < 0)
                        issues.Add("RetryCount configuration must be a non-negative integer");
                }
            }

            // Validate dos and don'ts
            if (implementation.DosAndDonts == null || !implementation.DosAndDonts.Any())
                issues.Add("At least one DO/DON'T guideline is required");
            else if (implementation.DosAndDonts.Count < _validationRules.MinimumDosAndDonts)
                issues.Add($"At least {_validationRules.MinimumDosAndDonts} DO/DON'T guidelines are required");
            else if (implementation.DosAndDonts.Count > _validationRules.MaximumDosAndDonts)
                issues.Add($"Maximum {_validationRules.MaximumDosAndDonts} DO/DON'T guidelines allowed");

            if (implementation.DosAndDonts != null)
            {
                foreach (var guideline in implementation.DosAndDonts)
                {
                    if (!guideline.StartsWith("DO:", StringComparison.OrdinalIgnoreCase) &&
                        !guideline.StartsWith("DON'T:", StringComparison.OrdinalIgnoreCase))
                        issues.Add($"Guideline '{guideline}' must start with 'DO:' or 'DON'T:'");
                }
            }

            return issues;
        }

        private List<string> ValidateQualityIndicators(QualityIndicators indicators)
        {
            var issues = new List<string>();

            if (indicators == null)
            {
                issues.Add("Quality indicators are required");
                return issues;
            }

            // Validate repeatability score
            if (indicators.RepeatabilityScore < _validationRules.MinimumRepeatabilityScore ||
                indicators.RepeatabilityScore > 100)
                issues.Add($"Repeatability score must be between {_validationRules.MinimumRepeatabilityScore} and 100");

            // Validate learning curve
            if (!_validationRules.ValidLearningCurves.Contains(indicators.LearningCurve?.ToLowerInvariant()))
                issues.Add($"Learning curve must be one of: {string.Join(", ", _validationRules.ValidLearningCurves)}");

            // Validate maintenance cost
            if (!_validationRules.ValidMaintenanceCosts.Contains(indicators.MaintenanceCost?.ToLowerInvariant()))
                issues.Add($"Maintenance cost must be one of: {string.Join(", ", _validationRules.ValidMaintenanceCosts)}");

            return issues;
        }

        private List<string> ValidateAiAssistance(AiAssistance aiAssistance)
        {
            var issues = new List<string>();

            if (aiAssistance == null)
            {
                issues.Add("AI assistance details are required");
                return issues;
            }

            // Validate prompt templates
            if (aiAssistance.PromptTemplates == null || aiAssistance.PromptTemplates.Count < _validationRules.RequiredPromptTemplates)
                issues.Add($"At least {_validationRules.RequiredPromptTemplates} prompt templates are required");

            // Validate validation rules
            if (aiAssistance.ValidationRules == null || aiAssistance.ValidationRules.Count < _validationRules.RequiredValidationRules)
                issues.Add($"At least {_validationRules.RequiredValidationRules} validation rule is required");

            // Validate common pitfalls
            if (aiAssistance.CommonPitfalls == null || aiAssistance.CommonPitfalls.Count < _validationRules.MinimumCommonPitfalls)
                issues.Add($"At least {_validationRules.MinimumCommonPitfalls} common pitfalls are required");

            return issues;
        }

        private List<string> ValidateAdoptionMetrics(AdoptionMetrics metrics)
        {
            var issues = new List<string>();

            if (metrics == null)
            {
                issues.Add("Adoption metrics are required");
                return issues;
            }

            // Validate time save format
            if (string.IsNullOrWhiteSpace(metrics.EstimatedTimeSave))
                issues.Add("Estimated time save is required");
            else if (!Regex.IsMatch(metrics.EstimatedTimeSave, @"^\d{1,3}%(-\d{1,3}%)?$"))
                issues.Add("Estimated time save must be in format like '40-60%' or '50%'");

            // Validate error reduction format
            if (string.IsNullOrWhiteSpace(metrics.ErrorReduction))
                issues.Add("Error reduction estimate is required");
            else if (!Regex.IsMatch(metrics.ErrorReduction, @"^\d{1,3}%(-\d{1,3}%)?$"))
                issues.Add("Error reduction must be in format like '40-60%' or '50%'");

            // Validate team satisfaction
            if (metrics.TeamSatisfaction < 1 || metrics.TeamSatisfaction > _validationRules.MaximumTeamSatisfaction)
                issues.Add($"Team satisfaction must be between 1 and {_validationRules.MaximumTeamSatisfaction}");

            return issues;
        }

        private async Task<List<string>> ValidateBusinessRules(TestingPatternDto pattern)
        {
            var issues = new List<string>();

            // Simulate async business rule validation
            await Task.Delay(50);

            // Rule 1: Pattern name should reflect area
            if (!pattern.Name.Contains(pattern.Area.ToLowerInvariant().Replace(" ", "-")))
                issues.Add($"Pattern name should include the area '{pattern.Area}'");

            // Rule 2: High repeatability score should have more code examples
            if (pattern.QualityIndicators.RepeatabilityScore >= 80 &&
                pattern.Implementation.CodeExamples.Count < 4)
                issues.Add("High repeatability patterns should have at least 4 code examples");

            // Rule 3: Steep learning curve patterns need more pitfalls documented
            if (pattern.QualityIndicators.LearningCurve == "steep" &&
                pattern.AiAssistance.CommonPitfalls.Count < 5)
                issues.Add("Steep learning curve patterns should document at least 5 common pitfalls");

            // Rule 4: High maintenance cost patterns need detailed dos/donts
            if (pattern.QualityIndicators.MaintenanceCost == "high" &&
                pattern.Implementation.DosAndDonts.Count < 6)
                issues.Add("High maintenance patterns should have at least 6 DO/DON'T guidelines");

            return issues;
        }
    }

    // Configuration class for validation rules
    public class ValidationRules
    {
        public int MinimumCodeExamples { get; set; }
        public int MaximumCodeExamples { get; set; }
        public List<string> RequiredConfigKeys { get; set; }
        public string ValidPatternNameRegex { get; set; }
        public int MinimumDosAndDonts { get; set; }
        public int MaximumDosAndDonts { get; set; }
        public List<string> ValidLearningCurves { get; set; }
        public List<string> ValidMaintenanceCosts { get; set; }
        public int MinimumRepeatabilityScore { get; set; }
        public int MaximumTeamSatisfaction { get; set; }
        public int RequiredPromptTemplates { get; set; }
        public int RequiredValidationRules { get; set; }
        public int MinimumCommonPitfalls { get; set; }
    }
}