
// Models/Responses/BiasAuditErrorResponse.cs
namespace Chapter_12.Models.Errors
{
    public class BiasAuditErrorResponse
    {
        public string Message { get; set; }
        public string DatasetName { get; set; }
        public string ErrorType { get; set; }
        public string SuggestedRemediation { get; set; }
        public string ErrorId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}