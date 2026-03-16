

using Chapter_6.Interfaces.BDDSupercharged.Interfaces;
using Chapter_6.Models;
using Chapter_6.Models.Requests;
using Chapter_6.Models.Responses;
using System.Text;

namespace Chapter_6.Interfaces.Implementations
{
 
        public class ScenarioTranslator : IScenarioTranslator
        {
            private readonly ILogger<ScenarioTranslator> _logger;

            public ScenarioTranslator(ILogger<ScenarioTranslator> logger)
            {
                _logger = logger;
            }

        public async Task<TranslatedAutomationStep> TranslateStepAsync(string step, TechContext context, string translationStyle)
        {
            _logger.LogDebug("Translating step: {Step} for stack {Stack}",
                step, context.Stack);

            // Parse step to determine type
            var stepType = DetermineStepType(step);
            var code = GenerateCodeForStep(step, stepType, context, translationStyle);
            var validationRules = GenerateValidationRules(step, stepType);
            var dependencies = DetermineDependencies(stepType, context);

            return new TranslatedAutomationStep
            {
                Code = code,
                ValidationRules = validationRules,
                Dependencies = dependencies
            };
        }


        public async Task<FrameworkIntegration> GenerateFrameworkIntegrationAsync(AutomationStep[] steps, TechContext context)
        {
            _logger.LogInformation("Generating framework integration for {StepCount} steps",
                steps.Length);

            var frameworkSetup = GenerateFrameworkSetup(context);
            var testStructure = GenerateTestStructure(steps, context);
            var requiredPackages = DetermineRequiredPackages(context);
            var configuration = GenerateConfiguration(steps, context);

            return new FrameworkIntegration
            {
                FrameworkSetup = frameworkSetup,
                TestStructure = testStructure,
                RequiredPackages = requiredPackages,
                Configuration = configuration
            };
        }

        //public async Task<FrameworkIntegration> GenerateFrameworkIntegrationAsync(AutomationStep[] steps, TechContext context)
        //{
        //    _logger.LogInformation("Generating framework integration for {StepCount} steps",
        //        steps.Length);

        //    var frameworkSetup = GenerateFrameworkSetup(context);
        //    var testStructure = GenerateTestStructure(steps, context);
        //    var requiredPackages = DetermineRequiredPackages(context);
        //    var configuration = GenerateConfiguration(steps, context);

        //    return new FrameworkIntegration
        //    {
        //        FrameworkSetup = frameworkSetup,
        //        TestStructure = testStructure,
        //        RequiredPackages = requiredPackages,
        //        Configuration = configuration
        //    };
        //}

        private string DetermineStepType(string step)
            {
                if (step.StartsWith("Given", StringComparison.OrdinalIgnoreCase))
                    return "setup";
                else if (step.StartsWith("When", StringComparison.OrdinalIgnoreCase))
                    return "action";
                else if (step.StartsWith("Then", StringComparison.OrdinalIgnoreCase))
                    return "assertion";
                else if (step.StartsWith("And", StringComparison.OrdinalIgnoreCase))
                    return "continuation";
                else
                    return "unknown";
            }

            private string GenerateCodeForStep(string step, string stepType, TechContext context, string translationStyle)
            {
                return context.Stack.ToLower() switch
                {
                    "dotnet" => GenerateDotNetCode(step, stepType, translationStyle),
                    "javascript" => GenerateJavaScriptCode(step, stepType, translationStyle),
                    "python" => GeneratePythonCode(step, stepType, translationStyle),
                    _ => GenerateGenericCode(step, stepType, translationStyle)
                };
            }

            private string GenerateDotNetCode(string step, string stepType, string translationStyle)
            {
                var methodName = GenerateMethodName(step);
                return translationStyle switch
                {
                    "declarative" => $@"[StepDefinition(""{step}"")]
public void {methodName}()
{{
    // {stepType.ToUpperInvariant()}: {step}
    // Auto-generated implementation
}}",
                    "fluent" => $@"public void {methodName}() 
    => Given(""{step}"")
       .When(""performing action"")
       .Then(""verifying outcome"");",
                    _ => $@"public void {methodName}()
{{
    // {step}
    throw new NotImplementedException();
}}"
                };
            }

            private ValidationRule[] GenerateValidationRules(string step, string stepType)
            {
                var rules = new List<ValidationRule>();

                switch (stepType)
                {
                    case "setup":
                        rules.Add(new ValidationRule
                        {
                            Name = "SetupComplete",
                            Condition = "All dependencies initialized",
                            ErrorMessage = "Setup failed to initialize dependencies"
                        });
                        break;
                    case "action":
                        rules.Add(new ValidationRule
                        {
                            Name = "ActionExecutable",
                            Condition = "Action can be performed",
                            ErrorMessage = "Action cannot be executed"
                        });
                        break;
                    case "assertion":
                        rules.Add(new ValidationRule
                        {
                            Name = "AssertionValid",
                            Condition = "Expected outcome achieved",
                            ErrorMessage = "Assertion failed"
                        });
                        break;
                }

                return rules.ToArray();
            }

            private string[] DetermineDependencies(string stepType, TechContext context)
            {
                var dependencies = new List<string>();

                switch (stepType)
                {
                    case "setup":
                        dependencies.Add("TestFramework");
                        dependencies.Add("MockingLibrary");
                        break;
                    case "action":
                        dependencies.Add("BusinessLogic");
                        dependencies.Add("DataAccess");
                        break;
                    case "assertion":
                        dependencies.Add("AssertionLibrary");
                        break;
                }

                // Add context-specific dependencies
                dependencies.AddRange(context.Libraries);

                return dependencies.ToArray();
            }

            private string GenerateFrameworkSetup(TechContext context)
            {
                return context.TestFramework.ToLower() switch
                {
                    "xunit" => @"[Collection(""BDD Tests"")]
public class BDDTestCollection : IClassFixture<TestFixture>
{
    // xUnit setup for BDD tests
}",
                    "nunit" => @"[TestFixture]
[Category(""BDD"")]
public class BDDTests
{
    [SetUp]
    public void Setup() { }
}",
                    "mstest" => @"[TestClass]
public class BDDTests
{
    [TestInitialize]
    public void Initialize() { }
}",
                    _ => "// Framework setup"
                };
            }

            private string GenerateTestStructure(AutomationStep[] steps, TechContext context)
            {
                var structure = new StringBuilder();
                structure.AppendLine($"// Test structure for {context.Stack} using {context.TestFramework}");
                structure.AppendLine("// Generated from BDD scenarios");
                structure.AppendLine();

                foreach (var step in steps)
                {
                    structure.AppendLine($"// Step: {step.Code}");
                }

                return structure.ToString();
            }

            private string[] DetermineRequiredPackages(TechContext context)
            {
                var packages = new List<string>
            {
                context.TestFramework,
                "BDDFramework",
                "AssertionLibrary"
            };

                packages.AddRange(context.Libraries);

                return packages.ToArray();
            }

            private string[] GenerateConfiguration(AutomationStep[] steps, TechContext context)
            {
                return new[]
                {
                $"Stack: {context.Stack}",
                $"TestFramework: {context.TestFramework}",
                $"StepCount: {steps.Length}",
                $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
            };
            }

            private string GenerateMethodName(string step)
            {
                // Convert step to valid method name
                var cleaned = step
                    .Replace("Given ", "")
                    .Replace("When ", "")
                    .Replace("Then ", "")
                    .Replace("And ", "")
                    .Replace(" ", "_")
                    .Replace("-", "_")
                    .Replace(".", "")
                    .Replace(",", "");

                return "Step_" + cleaned;
            }

            private string GenerateJavaScriptCode(string step, string stepType, string translationStyle)
            {
                var functionName = GenerateMethodName(step);
                return $@"{translationStyle}('{step}', async function() {{
    // {stepType}: {step}
    // JavaScript implementation
}});";
            }

            private string GeneratePythonCode(string step, string stepType, string translationStyle)
            {
                var functionName = GenerateMethodName(step);
                return $@"@{translationStyle}('{step}')
def {functionName}():
    '''{stepType}: {step}'''
    # Python implementation";
            }

            private string GenerateGenericCode(string step, string stepType, string translationStyle)
            {
                return $@"// {stepType.ToUpperInvariant()}: {step}
// Generic implementation for unknown stack
// Translation style: {translationStyle}";
            }

    }
    
}
