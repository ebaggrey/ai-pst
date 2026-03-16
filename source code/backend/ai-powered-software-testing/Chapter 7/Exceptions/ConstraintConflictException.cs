using Chapter_7.Models.Responses;

namespace Chapter_7.Exceptions
{
    public class ConstraintConflictException : Exception
    {
        public ConflictingConstraint[] ConflictingConstraints { get; set; }
        public string[] ResolvableConstraints { get; set; }

        public ConstraintConflictException(
            string message,
            ConflictingConstraint[] conflictingConstraints,
            string[] resolvableConstraints)
            : base(message)
        {
            ConflictingConstraints = conflictingConstraints;
            ResolvableConstraints = resolvableConstraints;
        }
    }
}
