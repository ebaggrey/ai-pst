namespace Chapter_7.Exceptions
{
   
    public class PipelineComplexityException : Exception
    {
        public string[] ComplexStages { get; set; }

        public PipelineComplexityException(string message, string[] complexStages)
            : base(message)
        {
            ComplexStages = complexStages;
        }
    }
}