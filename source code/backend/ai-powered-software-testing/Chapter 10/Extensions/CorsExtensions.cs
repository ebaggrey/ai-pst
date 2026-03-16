
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Chapter_10.Extensions
{
    public static class CorsExtensions
    {
        public static CorsPolicyBuilder WithOriginsIfProvided(
            this CorsPolicyBuilder policy,
            string[]? origins,
            out bool hasSpecificOrigins)
        {
            hasSpecificOrigins = false;

            if (origins != null && origins.Length > 0)
            {
                var validOrigins = origins.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
                if (validOrigins.Length > 0)
                {
                    policy.WithOrigins(validOrigins);
                    hasSpecificOrigins = true;
                    return policy;
                }
            }

            policy.AllowAnyOrigin();
            return policy;
        }

        public static CorsPolicyBuilder WithMethodsIfProvided(
            this CorsPolicyBuilder policy,
            string[]? methods)
        {
            if (methods != null && methods.Length > 0)
            {
                var validMethods = methods.Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
                if (validMethods.Length > 0)
                {
                    return policy.WithMethods(validMethods);
                }
            }

            return policy.AllowAnyMethod();
        }

        public static CorsPolicyBuilder WithHeadersIfProvided(
            this CorsPolicyBuilder policy,
            string[]? headers)
        {
            if (headers != null && headers.Length > 0)
            {
                var validHeaders = headers.Where(h => !string.IsNullOrWhiteSpace(h)).ToArray();
                if (validHeaders.Length > 0)
                {
                    return policy.WithHeaders(validHeaders);
                }
            }

            return policy.AllowAnyHeader();
        }

        public static CorsPolicyBuilder ConfigureCredentials(
            this CorsPolicyBuilder policy,
            bool hasSpecificOrigins)
        {
            if (hasSpecificOrigins)
            {
                return policy.AllowCredentials();
            }

            return policy.DisallowCredentials();
        }
    }
}
