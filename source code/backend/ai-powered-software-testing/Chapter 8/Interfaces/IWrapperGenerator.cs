
using Chapter_8.Models.Requests;
using Chapter_8.Models.Responses;


namespace Chapter_8.Interfaces
{
    public interface IWrapperGenerator
    {
        Task<GeneratedWrapper> GenerateWrapperAsync(
            ModuleAnalysis moduleAnalysis,
            string wrapperType,
            SafetyMeasure[] safetyMeasures);
    }

    public class ModuleAnalysis
    {
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public InterfaceDefinition[] Interfaces { get; set; }
        public int ComplexityScore { get; set; }
        public string[] Dependencies { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class InterfaceDefinition
    {
        public string Name { get; set; }
        public MethodSignature[] Methods { get; set; }
        public string[] ReturnTypes { get; set; }
        public bool IsPublic { get; set; }
    }

    public class MethodSignature
    {
        public string Name { get; set; }
        public Parameter[] Parameters { get; set; }
        public string ReturnType { get; set; }
        public string[] Exceptions { get; set; }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsOptional { get; set; }
        public object DefaultValue { get; set; }
    }
}