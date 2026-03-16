

// Models/Requests/TestDataBiasAuditRequest.cs
namespace Chapter_12.Models.Requests
//{
{
    public class TestDataBiasAuditRequest
    {
        public string DatasetName { get; set; }
        public List<Dictionary<string, object>> DataSample { get; set; }
        public string DataContext { get; set; }
        public int SuggestionCount { get; set; }
        public string AIPrompt { get; set; }
    }
}