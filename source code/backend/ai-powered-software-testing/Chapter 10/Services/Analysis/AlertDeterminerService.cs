
// Services/Analysis/AlertDeterminerService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;

namespace Chapter_10.Services.Analysis
{
    public class AlertDeterminerService : IAlertDeterminer
    {
        private readonly ILogger<AlertDeterminerService> _logger;

        public AlertDeterminerService(ILogger<AlertDeterminerService> logger)
        {
            _logger = logger;
        }

        public async Task<AlertStatus> DetermineAlertStatusAsync(
            HealthScore healthScore,
            HistoricalBaseline[] baselines)
        {
            _logger.LogInformation("Determining alert status");

            if (healthScore == null)
            {
                return new AlertStatus
                {
                    Level = "unknown",
                    Alerts = new[] { "Unable to determine alert status" },
                    RecommendedAction = "Check data availability"
                };
            }

            string level = DetermineAlertLevel(healthScore.OverallScore);
            var alerts = GenerateAlerts(healthScore, level);
            string action = DetermineRecommendedAction(level);

            return await Task.FromResult(new AlertStatus
            {
                Level = level,
                Alerts = alerts,
                RecommendedAction = action
            });
        }

        private string DetermineAlertLevel(double score)
        {
            return score switch
            {
                < 40 => "critical",
                < 60 => "warning",
                < 80 => "info",
                _ => "healthy"
            };
        }

        private string[] GenerateAlerts(HealthScore healthScore, string level)
        {
            var alerts = new List<string>();

            if (level == "critical")
                alerts.Add("Immediate attention required - health score critically low");

            if (level == "warning")
                alerts.Add("Health score below target - action recommended");

            if (healthScore.ComponentScores != null)
            {
                foreach (var component in healthScore.ComponentScores.Where(c => c.Score < 50))
                {
                    alerts.Add($"Component '{component.ComponentName}' is underperforming");
                }
            }

            return alerts.ToArray();
        }

        private string DetermineRecommendedAction(string level)
        {
            return level switch
            {
                "critical" => "Initiate emergency response plan",
                "warning" => "Schedule review meeting",
                "info" => "Monitor closely",
                "healthy" => "Continue current practices",
                _ => "Review system status"
            };
        }

        Task<AlertStatus> IAlertDeterminer.DetermineAlertStatusAsync(HealthScore healthScore, HistoricalBaseline[] baselines)
        {
            throw new NotImplementedException();
        }
    }
}
