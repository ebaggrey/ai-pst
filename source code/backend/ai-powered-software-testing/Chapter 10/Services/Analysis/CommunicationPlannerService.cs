
// Services/Analysis/CommunicationPlannerService.cs
using Chapter_10.Interfaces;
using Chapter_10.Models.Requests;
using Chapter_10.Models.Responses;


namespace Chapter_10.Services.Analysis
{
    public class CommunicationPlannerService : ICommunicationPlanner
    {
        private readonly ILogger<CommunicationPlannerService> _logger;

        public CommunicationPlannerService(ILogger<CommunicationPlannerService> logger)
        {
            _logger = logger;
        }

        public async Task<CommunicationPlan> GenerateCommunicationPlanAsync(
            ActionableInsight[] insights,
            InsightContext context)
        {
            _logger.LogInformation("Generating communication plan for {InsightCount} insights",
                insights?.Length ?? 0);

            var stakeholders = DetermineStakeholders(insights, context);
            var message = GenerateMessage(insights);
            var channel = DetermineCommunicationChannel(insights);
            var timeline = DetermineTimeline(insights);

            return await Task.FromResult(new CommunicationPlan
            {
                Stakeholders = stakeholders,
                Message = message,
                Channel = channel,
                Timeline = timeline
            });
        }

        private string[] DetermineStakeholders(ActionableInsight[] insights, InsightContext context)
        {
            var stakeholders = new List<string>();

            if (context?.Stakeholders != null)
                stakeholders.AddRange(context.Stakeholders);

            if (insights != null)
            {
                foreach (var insight in insights)
                {
                    if (insight.Priority == "high")
                        stakeholders.Add("Executive Team");

                    if (insight.Type == "anomaly")
                        stakeholders.Add("Operations Team");

                    if (insight.Type == "trend")
                        stakeholders.Add("Strategy Team");
                }
            }

            return stakeholders.Distinct().ToArray();
        }

        private string GenerateMessage(ActionableInsight[] insights)
        {
            if (insights == null || insights.Length == 0)
                return "No actionable insights to communicate";

            var highPriorityCount = insights.Count(i => i.Priority == "high");
            var topInsight = insights.OrderByDescending(i => i.Impact).FirstOrDefault();

            return $"Identified {insights.Length} actionable insights including {highPriorityCount} high-priority items. " +
                   $"Top insight: {topInsight?.Title} with estimated impact of {topInsight?.Impact:P1}.";
        }

        private string DetermineCommunicationChannel(ActionableInsight[] insights)
        {
            if (insights == null || insights.Length == 0)
                return "Email";

            var hasCritical = insights.Any(i => i.Priority == "high" && i.Impact > 0.8);

            return hasCritical ? "Emergency Meeting" : "Weekly Review";
        }

        private DateTime DetermineTimeline(ActionableInsight[] insights)
        {
            if (insights == null || insights.Length == 0)
                return DateTime.UtcNow.AddDays(7);

            var hasUrgent = insights.Any(i => i.Priority == "high" && i.Type == "anomaly");

            return hasUrgent ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow.AddDays(3);
        }

      
    }
}