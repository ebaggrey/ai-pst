

using Chapter_6.Exceptions;
using Chapter_6.Interfaces.BDDSupercharged.Interfaces;
using Chapter_6.Models.Requests;
using Chapter_6.Models.Responses;
using System.Text;
using System.Text.Json;


namespace Chapter_6.Interfaces.Implementations
{
    public class LivingDocumentationGenerator : ILivingDocumentationGenerator
    {
        private readonly ILogger<LivingDocumentationGenerator> _logger;

        public LivingDocumentationGenerator(ILogger<LivingDocumentationGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<GeneratedDocumentation> GenerateDocumentationAsync(
            BDDScenario[] scenarios,
            TestResult[] testResults,
            Audience audience,
            string format,
            string[] include)
        {
            _logger.LogInformation(
                "Generating {Format} documentation for {ScenarioCount} scenarios for {Audience} audience",
                format, scenarios?.Length ?? 0, audience?.Role ?? "unknown");

            try
            {
                ValidateInputs(scenarios, audience, format);

                var content = format.ToLower() switch
                {
                    "html" => await GenerateHtmlDocumentationAsync(scenarios, testResults, audience, include),
                    "markdown" => await GenerateMarkdownDocumentationAsync(scenarios, testResults, audience, include),
                    "json" => await GenerateJsonDocumentationAsync(scenarios, testResults, audience, include),
                    _ => throw new AudienceMismatchException(
                        $"Unsupported documentation format: {format}",
                        audience.Role,
                        new[] { "html", "markdown", "json" })
                };

                var sections = ExtractSections(content, format);
                var navigation = GenerateNavigation(scenarios, format, audience);

                return new GeneratedDocumentation
                {
                    Format = format,
                    Content = content,
                    Sections = sections,
                    Navigation = navigation,
                    InteractiveElements = DetermineInteractiveElements(audience, format, include)
                };
            }
            catch (AudienceMismatchException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate {Format} documentation", format);
                throw new AudienceMismatchException(
                    $"Cannot generate {format} documentation for {audience?.Role ?? "unknown"} audience",
                    audience?.Role ?? "unknown",
                    new[] { format },
                    ex);
            }
        }

        private void ValidateInputs(BDDScenario[] scenarios, Audience audience, string format)
        {
            if (scenarios == null || scenarios.Length == 0)
                throw new ArgumentException("At least one scenario is required", nameof(scenarios));

            if (audience == null)
                throw new ArgumentNullException(nameof(audience));

            if (string.IsNullOrWhiteSpace(audience.Role))
                throw new ArgumentException("Audience role is required", nameof(audience));

            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException("Documentation format is required", nameof(format));
        }

        private async Task<string> GenerateHtmlDocumentationAsync(
            BDDScenario[] scenarios,
            TestResult[] testResults,
            Audience audience,
            string[] include)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine($"    <title>BDD Documentation - {audience.Role}</title>");
            html.AppendLine("    <style>");
            html.AppendLine(GenerateCssStyles(audience));
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            // Header
            html.AppendLine("    <header>");
            html.AppendLine($"        <h1>BDD Living Documentation</h1>");
            html.AppendLine($"        <p>Audience: {audience.Role} | Scenarios: {scenarios.Length}</p>");
            html.AppendLine($"        <p>Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</p>");
            html.AppendLine("    </header>");

            // Main content
            html.AppendLine("    <div class=\"container\">");

            // Summary
            html.AppendLine("    <section class=\"summary\">");
            html.AppendLine("        <h2>Summary</h2>");
            html.AppendLine($"        <p>Total Scenarios: {scenarios.Length}</p>");

            if (testResults != null && testResults.Length > 0)
            {
                var passedTests = testResults.Count(tr => tr.Passed);
                html.AppendLine($"        <p>Test Pass Rate: {passedTests}/{testResults.Length} ({(double)passedTests / testResults.Length:P0})</p>");
            }
            html.AppendLine("    </section>");

            // Scenarios
            html.AppendLine("    <section class=\"scenarios\">");
            html.AppendLine("        <h2>Scenarios</h2>");

            foreach (var scenario in scenarios)
            {
                html.AppendLine("        <div class=\"scenario\">");
                html.AppendLine($"            <h3>{scenario.Title}</h3>");

                if (!string.IsNullOrWhiteSpace(scenario.Description))
                {
                    html.AppendLine($"            <p class=\"description\">{scenario.Description}</p>");
                }

                // Tags
                if (scenario.Tags != null && scenario.Tags.Length > 0)
                {
                    html.AppendLine("            <div class=\"tags\">");
                    foreach (var tag in scenario.Tags)
                    {
                        html.AppendLine($"                <span class=\"tag\">{tag}</span>");
                    }
                    html.AppendLine("            </div>");
                }

                // Steps
                if (scenario.Given != null && scenario.Given.Length > 0)
                {
                    html.AppendLine("            <h4>Given</h4>");
                    html.AppendLine("            <ul>");
                    foreach (var given in scenario.Given)
                    {
                        html.AppendLine($"                <li>{given}</li>");
                    }
                    html.AppendLine("            </ul>");
                }

                if (scenario.When != null && scenario.When.Length > 0)
                {
                    html.AppendLine("            <h4>When</h4>");
                    html.AppendLine("            <ul>");
                    foreach (var when in scenario.When)
                    {
                        html.AppendLine($"                <li>{when}</li>");
                    }
                    html.AppendLine("            </ul>");
                }

                if (scenario.Then != null && scenario.Then.Length > 0)
                {
                    html.AppendLine("            <h4>Then</h4>");
                    html.AppendLine("            <ul>");
                    foreach (var then in scenario.Then)
                    {
                        html.AppendLine($"                <li>{then}</li>");
                    }
                    html.AppendLine("            </ul>");
                }

                // Examples
                if (include.Contains("examples") && scenario.Examples != null && scenario.Examples.Length > 0)
                {
                    html.AppendLine("            <h4>Examples</h4>");
                    html.AppendLine("            <ul>");
                    foreach (var example in scenario.Examples)
                    {
                        html.AppendLine($"                <li>{example}</li>");
                    }
                    html.AppendLine("            </ul>");
                }

                html.AppendLine("        </div>");
            }

            html.AppendLine("    </section>");

            // Test Results
            if (include.Contains("test-results") && testResults != null && testResults.Length > 0)
            {
                html.AppendLine("    <section class=\"test-results\">");
                html.AppendLine("        <h2>Test Results</h2>");
                html.AppendLine("        <table>");
                html.AppendLine("            <tr><th>Scenario</th><th>Status</th><th>Duration</th></tr>");

                foreach (var result in testResults)
                {
                    var statusClass = result.Passed ? "passed" : "failed";
                    html.AppendLine("            <tr>");
                    html.AppendLine($"                <td>{result.ScenarioId}</td>");
                    html.AppendLine($"                <td class=\"{statusClass}\">{(result.Passed ? "PASS" : "FAIL")}</td>");
                    html.AppendLine($"                <td>{result.Duration:F2}s</td>");
                    html.AppendLine("            </tr>");
                }

                html.AppendLine("        </table>");
                html.AppendLine("    </section>");
            }

            html.AppendLine("    </div>");
            html.AppendLine("    <footer>");
            html.AppendLine("        <p>BDD Living Documentation - Automatically Generated</p>");
            html.AppendLine("    </footer>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            await Task.Delay(10);
            return html.ToString();
        }

        private async Task<string> GenerateMarkdownDocumentationAsync(
            BDDScenario[] scenarios,
            TestResult[] testResults,
            Audience audience,
            string[] include)
        {
            var md = new StringBuilder();

            md.AppendLine($"# BDD Living Documentation");
            md.AppendLine();
            md.AppendLine($"*Audience: {audience.Role}*");
            md.AppendLine($"*Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*");
            md.AppendLine();

            md.AppendLine("## Summary");
            md.AppendLine();
            md.AppendLine($"- Total Scenarios: {scenarios.Length}");

            if (testResults != null && testResults.Length > 0)
            {
                var passedTests = testResults.Count(tr => tr.Passed);
                md.AppendLine($"- Test Pass Rate: {(double)passedTests / testResults.Length:P0}");
            }
            md.AppendLine();

            md.AppendLine("## Scenarios");
            md.AppendLine();

            foreach (var scenario in scenarios)
            {
                md.AppendLine($"### {scenario.Title}");
                md.AppendLine();

                if (!string.IsNullOrWhiteSpace(scenario.Description))
                {
                    md.AppendLine($"{scenario.Description}");
                    md.AppendLine();
                }

                if (scenario.Tags != null && scenario.Tags.Length > 0)
                {
                    md.AppendLine($"**Tags:** {string.Join(", ", scenario.Tags)}");
                    md.AppendLine();
                }

                if (scenario.Given != null && scenario.Given.Length > 0)
                {
                    md.AppendLine("#### Given");
                    foreach (var given in scenario.Given)
                    {
                        md.AppendLine($"- {given}");
                    }
                    md.AppendLine();
                }

                if (scenario.When != null && scenario.When.Length > 0)
                {
                    md.AppendLine("#### When");
                    foreach (var when in scenario.When)
                    {
                        md.AppendLine($"- {when}");
                    }
                    md.AppendLine();
                }

                if (scenario.Then != null && scenario.Then.Length > 0)
                {
                    md.AppendLine("#### Then");
                    foreach (var then in scenario.Then)
                    {
                        md.AppendLine($"- {then}");
                    }
                    md.AppendLine();
                }
            }

            if (include.Contains("test-results") && testResults != null && testResults.Length > 0)
            {
                md.AppendLine("## Test Results");
                md.AppendLine();
                md.AppendLine("| Scenario | Status | Duration |");
                md.AppendLine("|----------|--------|----------|");

                foreach (var result in testResults)
                {
                    var status = result.Passed ? "✅ PASS" : "❌ FAIL";
                    md.AppendLine($"| {result.ScenarioId} | {status} | {result.Duration:F2}s |");
                }
            }

            await Task.Delay(10);
            return md.ToString();
        }

        private async Task<string> GenerateJsonDocumentationAsync(
            BDDScenario[] scenarios,
            TestResult[] testResults,
            Audience audience,
            string[] include)
        {
            var documentation = new
            {
                Metadata = new
                {
                    GeneratedAt = DateTime.UtcNow,
                    Audience = new
                    {
                        audience.Role,
                        audience.TechnicalLevel,
                        audience.Interests,
                        audience.Constraints
                    },
                    Format = "json",
                    ScenarioCount = scenarios.Length,
                    IncludedSections = include
                },
                Statistics = new
                {
                    TotalScenarios = scenarios.Length,
                    TotalSteps = scenarios.Sum(s =>
                        (s.Given?.Length ?? 0) +
                        (s.When?.Length ?? 0) +
                        (s.Then?.Length ?? 0)),
                    TestResults = testResults != null ? new
                    {
                        TotalTests = testResults.Length,
                        PassedTests = testResults.Count(tr => tr.Passed),
                        PassRate = testResults.Length > 0 ?
                            (double)testResults.Count(tr => tr.Passed) / testResults.Length : 0
                    } : null
                },
                Scenarios = scenarios.Select(s => new
                {
                    s.Title,
                    s.Description,
                    s.Tags,
                    s.Given,
                    s.When,
                    s.Then,
                    s.Examples
                }),
                TestResults = include.Contains("test-results") && testResults != null ?
                    testResults.Select(tr => new
                    {
                        tr.ScenarioId,
                        tr.Passed,
                        tr.Duration,
                        tr.ExecutionTime,
                        tr.Errors
                    }) : null
            };

            await Task.Delay(10);
            return JsonSerializer.Serialize(documentation, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        private string GenerateCssStyles(Audience audience)
        {
            return @"
                body {
                    font-family: Arial, sans-serif;
                    line-height: 1.6;
                    margin: 0;
                    padding: 20px;
                    background: #f5f5f5;
                }
                
                header {
                    background: #2c3e50;
                    color: white;
                    padding: 20px;
                    border-radius: 5px;
                    margin-bottom: 20px;
                }
                
                h1 {
                    margin: 0;
                    font-size: 2em;
                }
                
                .container {
                    max-width: 1200px;
                    margin: 0 auto;
                }
                
                section {
                    background: white;
                    padding: 20px;
                    margin-bottom: 20px;
                    border-radius: 5px;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                }
                
                h2 {
                    color: #2c3e50;
                    border-bottom: 2px solid #eee;
                    padding-bottom: 10px;
                }
                
                .scenario {
                    border: 1px solid #ddd;
                    padding: 15px;
                    margin-bottom: 15px;
                    border-radius: 5px;
                }
                
                .scenario h3 {
                    margin-top: 0;
                    color: #3498db;
                }
                
                .tags {
                    margin: 10px 0;
                }
                
                .tag {
                    background: #ecf0f1;
                    padding: 3px 8px;
                    border-radius: 3px;
                    margin-right: 5px;
                    font-size: 0.9em;
                }
                
                table {
                    width: 100%;
                    border-collapse: collapse;
                }
                
                th, td {
                    border: 1px solid #ddd;
                    padding: 8px;
                    text-align: left;
                }
                
                th {
                    background: #f2f2f2;
                }
                
                .passed {
                    color: green;
                    font-weight: bold;
                }
                
                .failed {
                    color: red;
                    font-weight: bold;
                }
                
                footer {
                    text-align: center;
                    margin-top: 20px;
                    color: #7f8c8d;
                }
                
                ul {
                    padding-left: 20px;
                }
                
                li {
                    margin-bottom: 5px;
                }
            ";
        }

        private string[] ExtractSections(string content, string format)
        {
            return format.ToLower() switch
            {
                "html" => new[] { "header", "summary", "scenarios", "test-results", "footer" },
                "markdown" => new[] { "summary", "scenarios", "test-results" },
                "json" => new[] { "metadata", "statistics", "scenarios", "testResults" },
                _ => new[] { "content" }
            };
        }

        private string[] GenerateNavigation(BDDScenario[] scenarios, string format, Audience audience)
        {
            if (format.ToLower() != "html")
                return Array.Empty<string>();

            return new[] { "#summary", "#scenarios", "#test-results" };
        }

        private string[] DetermineInteractiveElements(Audience audience, string format, string[] include)
        {
            var elements = new List<string>();

            if (format.ToLower() == "html")
            {
                elements.Add("responsive-layout");

                if (audience.TechnicalLevel != "beginner")
                {
                    elements.Add("print-friendly");
                }
            }

            return elements.ToArray();
        }
    }
}



