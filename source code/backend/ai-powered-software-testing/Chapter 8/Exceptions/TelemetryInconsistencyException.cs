
namespace Chapter_8.Exceptions
{
    public class TelemetryInconsistencyException : Exception
    {
        public string InconsistencyDetails { get; set; }
        public string[] DataQualityIssues { get; set; }

        public TelemetryInconsistencyException() : base() { }

        public TelemetryInconsistencyException(string message) : base(message) { }

        public TelemetryInconsistencyException(string message, Exception innerException)
            : base(message, innerException) { }

        public TelemetryInconsistencyException(string message, string inconsistencyDetails, string[] dataQualityIssues)
            : base(message)
        {
            InconsistencyDetails = inconsistencyDetails;
            DataQualityIssues = dataQualityIssues;
        }
    }
}
