
using Chapter_8.Exceptions;
using Chapter_8.Interfaces;
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;
using System.Text;

namespace LegacyConquest.Services
{
    public class WrapperGenerator : IWrapperGenerator
    {
        private readonly ILogger<WrapperGenerator> _logger;

        public WrapperGenerator(ILogger<WrapperGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<GeneratedWrapper> GenerateWrapperAsync(
            ModuleAnalysis moduleAnalysis,
            string wrapperType,
            SafetyMeasure[] safetyMeasures)
        {
            try
            {
                _logger.LogInformation("Generating {WrapperType} wrapper for module {ModuleName}",
                    wrapperType, moduleAnalysis.ModuleName);

                // Simulate generation work
                await System.Threading.Tasks.Task.Delay(800);

                // Check for complexity issues
                if (moduleAnalysis.ComplexityScore > 8 && wrapperType == "adapter")
                {
                    throw new WrapperComplexityException(
                        $"Module {moduleAnalysis.ModuleName} too complex for adapter wrapper",
                        moduleAnalysis.ModuleName,
                        new[] { "High cyclomatic complexity", "Too many interfaces" })
                    {
                        ModuleName = moduleAnalysis.ModuleName,
                        ComplexityIssues = new[] { "Cyclomatic complexity > 50", "15+ public methods" }
                    };
                }

                // Generate wrapper code based on type
                var wrapperCode = GenerateWrapperCode(moduleAnalysis, wrapperType, safetyMeasures);

                var wrapper = new GeneratedWrapper
                {
                    Name = $"{moduleAnalysis.ModuleName}Wrapper",
                    Code = wrapperCode,
                    Language = "C#",
                    SafetyFeatures = safetyMeasures.Select((s, i) => new SafetyFeature
                    {
                        Name = $"SafetyFeature_{i}",
                        Type = s.Type,
                        Configuration = s.Configuration
                    }).ToArray(),
                    ExposedInterfaces = moduleAnalysis.Interfaces?
                        .Where(i => i.IsPublic)
                        .Select(i => i.Name)
                        .ToArray() ?? Array.Empty<string>()
                };

                _logger.LogInformation("Wrapper generation complete for {ModuleName}",
                    moduleAnalysis.ModuleName);

                return wrapper;
            }
            catch (Exception ex) when (ex is not WrapperComplexityException)
            {
                _logger.LogError(ex, "Error generating wrapper for {ModuleName}",
                    moduleAnalysis.ModuleName);
                throw;
            }
        }

        private string GenerateWrapperCode(ModuleAnalysis moduleAnalysis, string wrapperType, SafetyMeasure[] safetyMeasures)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"// Generated {wrapperType} wrapper for {moduleAnalysis.ModuleName}");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"public class {moduleAnalysis.ModuleName}Wrapper");
            sb.AppendLine("{");
            sb.AppendLine($"    private readonly {moduleAnalysis.ModuleName} _legacyInstance;");
            sb.AppendLine("    private readonly ILogger _logger;");

            foreach (var safety in safetyMeasures)
            {
                sb.AppendLine($"    private readonly {safety.Type} _safety;");
            }

            sb.AppendLine();
            sb.AppendLine($"    public {moduleAnalysis.ModuleName}Wrapper()");
            sb.AppendLine("    {");
            sb.AppendLine($"        _legacyInstance = new {moduleAnalysis.ModuleName}();");
            sb.AppendLine("    }");

            // Generate wrapper methods for each interface
            if (moduleAnalysis.Interfaces != null)
            {
                foreach (var iface in moduleAnalysis.Interfaces)
                {
                    foreach (var method in iface.Methods ?? Array.Empty<MethodSignature>())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"    public async Task<{method.ReturnType}> {method.Name}Async(" +
                            string.Join(", ", method.Parameters?.Select(p => $"{p.Type} {p.Name}") ?? Array.Empty<string>()) + ")");
                        sb.AppendLine("    {");

                        // Add safety measures
                        if (safetyMeasures.Any(s => s.Type == "circuit-breaker"))
                        {
                            sb.AppendLine("        // Circuit breaker pattern");
                            sb.AppendLine("        if (_circuitBreaker.IsOpen)");
                            sb.AppendLine("            throw new CircuitOpenException();");
                        }

                        sb.AppendLine("        try");
                        sb.AppendLine("        {");
                        sb.AppendLine($"            var result = await Task.Run(() => _legacyInstance.{method.Name}(" +
                            string.Join(", ", method.Parameters?.Select(p => p.Name) ?? Array.Empty<string>()) + "));");
                        sb.AppendLine("            return result;");
                        sb.AppendLine("        }");
                        sb.AppendLine("        catch (Exception ex)");
                        sb.AppendLine("        {");
                        sb.AppendLine("            _logger.LogError(ex, \"Error in wrapper method\");");
                        sb.AppendLine("            throw new WrapperException(\"Error calling legacy method\", ex);");
                        sb.AppendLine("        }");
                        sb.AppendLine("    }");
                    }
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}