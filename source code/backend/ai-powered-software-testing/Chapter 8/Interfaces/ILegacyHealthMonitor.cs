
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;

namespace Chapter_8.Interfaces
{
    public interface ILegacyHealthMonitor
    {
        Task<HealthReport> MonitorHealthAsync(HealthRequest request);
    }

    public class HealthReport
    {
        public string ReportId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public Dictionary<string, double> CurrentHealthScores { get; set; }
        public Anomaly[] Anomalies { get; set; }
        public Trend[] Trends { get; set; }
        public Prediction[] Predictions { get; set; }
        public string[] Recommendations { get; set; }
    }
}