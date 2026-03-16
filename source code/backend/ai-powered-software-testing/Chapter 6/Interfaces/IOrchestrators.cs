
using Chapter_6.Models.Requests;
using Chapter_6.Models.Responses;
using Chapter_6.Models;
namespace Chapter_6.Interfaces
{


    // Interfaces/IOrchestrators.cs

    //public interface IBDDConversationOrchestrator
    //{
    //    Task<BDDConversation> StartConversationAsync(BDCCoCreationRequest request);
    //    Task<ConversationRound> FacilitateRoundAsync(BDDConversation conversation, int round, BDCCoCreationRequest request);
    //    Task<BDDScenario[]> SynthesizeScenariosAsync(BDDConversation conversation);
    //}

    //public interface IScenarioTranslator
    //{
    //    Task<TranslatedAutomationStep> TranslateStepAsync(string step, TechContext context, string translationStyle);
    //    Task<FrameworkIntegration> GenerateFrameworkIntegrationAsync(TranslatedAutomationStep[] steps, TechContext context);
    //}

    //public interface IScenarioEvolver
    //{
    //    Task<EvolutionResult> EvolveScenarioAsync(BDDScenario scenario, string newInformation, BreakingChange[] breakingChanges, string evolutionStrategy);
    //}

    //public interface IDriftDetector
    //{
    //    Task<DriftFinding[]> DetectDriftUsingMethodAsync(BDDScenario[] documentedScenarios, ImplementedBehavior[] implementedBehavior, string method, double sensitivity);
    //}

    //public interface ILivingDocumentationGenerator
    //{
    //    Task<GeneratedDocumentation> GenerateDocumentationAsync(BDDScenario[] scenarios, TestResult[] testResults, Audience audience, string format, string[] include);
    //}

    // Interfaces/IOrchestrators.cs
   

    namespace BDDSupercharged.Interfaces
    {
        public interface IBDDConversationOrchestrator
        {
            Task<BDDConversation> StartConversationAsync(BDCCoCreationRequest request);
            Task<ConversationRound> FacilitateRoundAsync(BDDConversation conversation, int round, BDCCoCreationRequest request);
            Task<BDDScenario[]> SynthesizeScenariosAsync(BDDConversation conversation);
        }

        public interface IScenarioTranslator
        {
            Task<TranslatedAutomationStep> TranslateStepAsync(string step, TechContext context, string translationStyle);
            Task<FrameworkIntegration> GenerateFrameworkIntegrationAsync(AutomationStep[] steps, TechContext context);
        }

        public interface IScenarioEvolver
        {
            Task<EvolutionResult> EvolveScenarioAsync(BDDScenario scenario, string newInformation, BreakingChange[] breakingChanges, string evolutionStrategy);
        }

        public interface IDriftDetector
        {
            Task<DriftFinding[]> DetectDriftUsingMethodAsync(BDDScenario[] documentedScenarios, ImplementedBehavior[] implementedBehavior, string method, double sensitivity);
        }

        public interface ILivingDocumentationGenerator
        {
            Task<GeneratedDocumentation> GenerateDocumentationAsync(BDDScenario[] scenarios, TestResult[] testResults, Audience audience, string format, string[] include);
        }
    }


}
