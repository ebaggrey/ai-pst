  using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace Chapter_2.Services.Interfaces
    {
       
        public interface ILLMServiceFactory
        {
            
            ILLMService GetService(string providerName);
            ILLMService GetServiceForTask(string task, string strategy, string context);
            IEnumerable<ILLMService> GetAllServices();
        }

        
    }

    /// <summary>
    /// Represents the health status of an LLM provider.
    /// </summary>
    public class ProviderHealth
    {
        public string ProviderName { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public long ResponseTime { get; set; } // ms
        public string? Error { get; set; }
        public DateTime LastChecked { get; set; }
    }

