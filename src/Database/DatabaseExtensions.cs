using FluentMigrator.Runner;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HomeAutomation.Database;

public static class DatabaseExtensions
{
    /// <summary>
    /// Adds the Home Automation database services.
    /// </summary>
    /// <param name="services">The apps service collection</param>
    /// <returns>Returns the updated service collection</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddHomeAutomationDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddLinqToDBContext<HomeAutomationDb>((provider, options) =>
        {
            // Get the connection string 
            string connectionString = config.GetConnectionString("HomeAutomationDb")
                ?? throw new InvalidOperationException("Connection string 'HomeAutomationDb' is not configured.");

            // Configure the database connection string
            options.UsePostgreSQL(LinqToDB.DataProvider.PostgreSQL.PostgreSQLVersion.AutoDetect)
                   .UseConnectionString(connectionString)
                   .UseDefaultLogging(provider);

            return options;
        });

        services.AddFluentMigratorCore()
                .ConfigureRunner(r =>
                {
                    // Get the connection string 
                    string connectionString = config.GetConnectionString("HomeAutomationDb")
                        ?? throw new InvalidOperationException("Connection string 'HomeAutomationDb' is not configured.");

                    r.AddPostgres()
                     .WithGlobalConnectionString(connectionString)
                     .ScanIn(typeof(HomeAutomation.Models.Database.Migrations.InitialMigration).Assembly);
                })
                .AddLogging(lb => lb.AddSerilog());

        return services;
    }

    /// <summary>
    /// Migrate the database to the latest version.
    /// </summary>
    /// <param name="serviceProvider">The app's ServiceProvider instance</param>
    /// <returns>Returns the Service Provider for fluent chaining</returns>
    public static IServiceProvider MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        return serviceProvider;
    }
}