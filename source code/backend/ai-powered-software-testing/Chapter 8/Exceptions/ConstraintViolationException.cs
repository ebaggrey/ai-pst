
namespace Chapter_8.Exceptions
{
    public class ConstraintViolationException : Exception
    {
        public string[] ConflictingConstraints { get; set; }
        public string[] ConstraintAdjustments { get; set; }

        public ConstraintViolationException() : base() { }

        public ConstraintViolationException(string message) : base(message) { }

        public ConstraintViolationException(string message, Exception innerException)
            : base(message, innerException) { }

        public ConstraintViolationException(string message, string[] conflictingConstraints, string[] constraintAdjustments)
            : base(message)
        {
            ConflictingConstraints = conflictingConstraints;
            ConstraintAdjustments = constraintAdjustments;
        }
    }
}
