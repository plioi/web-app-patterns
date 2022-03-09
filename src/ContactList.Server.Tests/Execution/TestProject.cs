using Fixie;

namespace ContactList.Server.Tests.Execution;

class TestProject : ITestProject
{
    public void Configure(TestConfiguration configuration, TestEnvironment environment)
    {
        configuration.Conventions.Add<DefaultDiscovery, ServerTestExecution>();

        if (environment.IsDevelopment())
            configuration.Reports.Add<DiffToolReport>();
    }
}
