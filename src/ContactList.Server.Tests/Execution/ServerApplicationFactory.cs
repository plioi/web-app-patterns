using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContactList.Server.Tests.Execution;

/// <summary>
/// ServerApplicationFactory provides a test-friendly instance of the ContactList.Server
/// application, so that tests can for instance access representative application config
/// settings and service scopes set up in Program.cs.
///
/// Test-specific overrides of the application config and services can also be defined here.
/// </summary>
class ServerApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((_, services) =>
        {
            // Test-Specific Service Configuration Overrides.

            //Example:
            //
            // services.AddSingleton<IExampleService, StubExampleService>();

            services.AddSingleton(this);
        });

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            // Test-Specific App Configuration Overrides.

            //Example:
            //
            // configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            //     {
            //         {"ConnectionStrings:Example", testSpecificConnectionString},
            //     });

            var testsDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            configBuilder.AddJsonFile(Path.Join(testsDirectory, "appsettings.json"));
        });
    }
}
