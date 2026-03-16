
using Chapter_11.Exceptions;
using Chapter_11.Interfaces;
using Chapter_11.Models.Analysis;
using Chapter_11.Models.Requests;
using Chapter_11.Services;


namespace Chapter_11.Orchestrators
{
    public class ShiftRightOrchestrator : IShiftRightOrchestrator
    {
        private readonly ILLMService _llmService;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<ShiftRightOrchestrator> _logger;

        public ShiftRightOrchestrator(
            ILLMService llmService,
            IDatabaseService databaseService,
            ILogger<ShiftRightOrchestrator> logger)
        {
            _llmService = llmService;
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<Models.Responses.Monitor[]> GenerateMonitorsAsync(
            ProductionSystemAnalysis systemAnalysis,
            UserBehaviorAnalysis behaviorAnalysis,
            MonitoringObjective[] objectives)
        {
            try
            {
                _logger.LogInformation("Generating monitors for {Count} objectives with {ComponentCount} components",
                    objectives.Length, systemAnalysis.Components?.Length ?? 0);

                // Check complexity
                if (systemAnalysis.Components != null && systemAnalysis.Components.Length > 50)
                {
                    throw new MonitoringComplexityException(
                        "System too complex for automated monitoring",
                        new[] { "Too many components: " + systemAnalysis.Components.Length, "Complex dependencies detected" },
                        new[] { "Focus on critical components first", "Start with core services", "Use phased monitoring implementation" }
                    );
                }

                var monitors = new List<Models.Responses.Monitor>();

                foreach (var objective in objectives)
                {
                    // Use LLM to generate monitor configuration based on analysis
                    var prompt = $"Generate monitor configuration for objective '{objective.Name}' with metric '{objective.Metric}' based on system analysis and user behavior patterns.";
                    var config = await _llmService.GenerateCompletionAsync(prompt);

                    monitors.Add(new Models.Responses.Monitor
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"Monitor for {objective.Name}",
                        Type = DetermineMonitorType(objective),
                        Configuration = config ?? "{}",
                        Targets = DetermineTargets(objective, systemAnalysis)
                    });
                }

                // Save monitors to database
                foreach (var monitor in monitors)
                {
                    await _databaseService.SaveAsync(monitor);
                }

                return monitors.ToArray();
            }
            catch (MonitoringComplexityException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate monitors");
                throw new MonitoringComplexityException(
                    "Unexpected error during monitor generation",
                    new[] { ex.Message },
                    new[] { "Try again with simplified requirements", "Contact support if issue persists" }
                );
            }
        }

        private string DetermineMonitorType(MonitoringObjective objective)
        {
            if (objective.Metric?.Contains("response") == true)
                return "Performance";
            if (objective.Metric?.Contains("error") == true)
                return "ErrorRate";
            if (objective.Metric?.Contains("user") == true)
                return "UserBehavior";
            return "Metric";
        }

        private string[] DetermineTargets(MonitoringObjective objective, ProductionSystemAnalysis systemAnalysis)
        {
            if (systemAnalysis.Components == null)
                return new[] { objective.Name };

            return systemAnalysis.Components
                .Where(c => c.Name?.Contains(objective.Name?.Split('.').Last() ?? "") == true)
                .Select(c => c.Id)
                .ToArray();
        }
    }
}
