using Fixie;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContactList.Server.Tests.Execution;

class ServerTestExecution : IExecution
{
    static IConfiguration? _configuration;
    static IServiceScopeFactory? _scopeFactory;

    public static IServiceScope CreateScope()
    {
        if (_scopeFactory == null)
            throw new InvalidOperationException(
                $"{nameof(CreateScope)} may only be called during a test run, " +
                "because it relies on the services prepared during" +
                $"{nameof(ServerTestExecution)}.{nameof(Run)}(...)");

        return _scopeFactory.CreateScope();
    }

    public async Task Run(TestSuite testSuite)
    {
        await using var factory = new ServerApplicationFactory();

        _configuration = factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();

        foreach (var test in testSuite.Tests)
            await test.Run();

        _configuration = null;
        _scopeFactory = null;
    }
}
