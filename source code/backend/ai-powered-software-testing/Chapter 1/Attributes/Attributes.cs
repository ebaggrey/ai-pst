using Chapter_1.Models;
using System.ComponentModel.DataAnnotations;

namespace Chapter_1.Attributes
{
    // Validation/ValidateRiskAssessmentAttribute.cs
    public class ValidateRiskAssessmentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is RiskAssessment riskAssessment)
            {
                var errors = new List<string>();

                if (riskAssessment.Criticality < 1 || riskAssessment.Criticality > 10)
                {
                    errors.Add("Criticality must be between 1 and 10");
                }

                if (riskAssessment.RiskFactors.Any(rf => rf.Likelihood < 1 || rf.Likelihood > 10))
                {
                    errors.Add("Risk factor likelihood must be between 1 and 10");
                }

                if (riskAssessment.RiskFactors.Any(rf => rf.Impact < 1 || rf.Impact > 10))
                {
                    errors.Add("Risk factor impact must be between 1 and 10");
                }

                if (errors.Any())
                {
                    return new ValidationResult(string.Join("; ", errors));
                }
            }

            return ValidationResult.Success;
        }
    }
}
