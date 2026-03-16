using Chapter_7.Data.Context;
using Chapter_7.Data.Repositories;
using Chapter_7.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
namespace Chapter_7.Extensions
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
        {
            // Add DbContext pool for better performance
            services.AddDbContextPool<PipelineDbContext>((serviceProvider, options) =>
            {
                var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>();
                var connectionString = connectionStrings.Value.PipelineDatabase;

                // Check if connection string is null or empty
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Database connection string is not configured.");
                }

                // UseSqlServer is from Microsoft.EntityFrameworkCore.SqlServer namespace
                // Make sure you have the package reference
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // Configure SQL Server specific options
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(60);
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");

                    // Use QuerySplittingBehavior for better performance with includes
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                // Optional: Enable sensitive data logging in development
#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine, LogLevel.Information);
#endif
            }, poolSize: 128); // Default pool size is 128

            services.AddScoped<IPipelineRepository, PipelineRepository>();

            return services;
        }

        // Alternative: If UseSqlServer still not found, use this approach with explicit provider
        public static IServiceCollection AddDatabaseServicesAlternative(this IServiceCollection services)
        {
            // Register DbContext with SQL Server using the provider explicitly
            services.AddDbContext<PipelineDbContext>((serviceProvider, options) =>
            {
                var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>();
                var connectionString = connectionStrings.Value.PipelineDatabase;

                // This is the fully qualified way to use SQL Server
                var sqlServerOptionsAction = (SqlServerDbContextOptionsBuilder sqlOptions) =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(60);
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
                };

                options.UseSqlServer(connectionString, sqlServerOptionsAction);
            });

            services.AddScoped<IPipelineRepository, PipelineRepository>();

            return services;
        }

        // ... other extension methods
    }
}
