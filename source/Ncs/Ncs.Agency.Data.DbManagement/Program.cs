using System;
using System.Data.Common;
using System.Linq;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ncs.Solicitor.Data.DbManagement.Migrations;
using Npgsql;

namespace Ncs.Solicitor.Data.DbManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            using var serviceProvider = CreateServices(args);
            using var scope = serviceProvider.CreateScope();

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            if (args[0] == "up")
            {
                UpdateDatabase(scope.ServiceProvider);
            }
            else if (args[0] == "downto")
            {
                DowngradeDatabase(scope.ServiceProvider, long.Parse(args[1]));
            }
            else if (args[0] == "wipe")
            {
                DowngradeDatabase(scope.ServiceProvider, 0);
            }
            else if (args[0] == "dev-reset")
            {
                DowngradeDatabase(scope.ServiceProvider, 0);
                UpdateDatabase(scope.ServiceProvider);
            }
            else
            {
                throw new ApplicationException($"Unknown command: {args[0]}");
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static ServiceProvider CreateServices(string[] args)
        {
            var connString = "Host=localhost;Username=ncsusr;Password=abc123;Database=postgres;";
            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(builder => builder
                    // Add DB support to FluentMigrator
                    .AddPostgres11_0()
                    // Set the connection string
                    .WithGlobalConnectionString(connString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(M202301280001_AddDebtCollectionCase).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateUp();
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void DowngradeDatabase(IServiceProvider serviceProvider, long version)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            runner.MigrateDown(version);
        }
    }
}