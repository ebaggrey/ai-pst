namespace Chapter_7.Exceptions
{
   
    public class AdaptationComplexityException : Exception
    {
        public int StepCount { get; set; }

        public AdaptationComplexityException(string message, int stepCount)
            : base(message)
        {
            StepCount = stepCount;
        }
    }
}