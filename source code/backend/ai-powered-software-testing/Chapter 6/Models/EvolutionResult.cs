using Chapter_6.Models.Requests;
using Chapter_6.Models.Responses;

namespace Chapter_6.Models
{

    public class EvolutionResult
    {
        public BDDScenario EvolvedScenario { get; set; } = new BDDScenario();
        public EvolutionRecord EvolutionRecord { get; set; } = new EvolutionRecord();
        public double PreservationScore { get; set; }
    }

    public class TranslatedAutomationStep
    {
        public string Code { get; set; } = string.Empty;
        public ValidationRule[] ValidationRules { get; set; } = Array.Empty<ValidationRule>();
        public string[] Dependencies { get; set; } = Array.Empty<string>();
    }

}
