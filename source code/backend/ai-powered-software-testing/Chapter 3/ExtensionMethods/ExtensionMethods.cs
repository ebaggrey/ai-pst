using Chapter_3.Models.Domain;
using System.Text.Json;

namespace Chapter_3.ExtensionMethods
{
    // Extensions/GeneratedTestExtensions.cs
    public static class GeneratedTestExtensions
    {
        public static GeneratedTest DeepClone(this GeneratedTest original)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            // Use JSON serialization for deep cloning to handle nested objects
            var json = JsonSerializer.Serialize(original);
            return JsonSerializer.Deserialize<GeneratedTest>(json) ?? throw new InvalidOperationException("Failed to clone GeneratedTest");
        }
    }
}
