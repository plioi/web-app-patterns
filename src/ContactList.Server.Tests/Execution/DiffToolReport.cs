using DiffEngine;
using Fixie.Reports;

namespace ContactList.Server.Tests.Execution;

class DiffToolReport : IHandler<TestFailed>, IHandler<ExecutionCompleted>
{
    int _failures;
    Exception? _singleFailure;

    public Task Handle(TestFailed message)
    {
        _failures++;

        _singleFailure = _failures == 1 ? message.Reason : null;

        return Task.CompletedTask;
    }

    public async Task Handle(ExecutionCompleted message)
    {
        if (_singleFailure is MatchException exception)
        {
            var tempPath = Path.GetTempPath();
            var expectedPath = Path.Combine(tempPath, "expected.txt");
            var actualPath = Path.Combine(tempPath, "actual.txt");

            File.WriteAllText(expectedPath, Json(exception.Expected));
            File.WriteAllText(actualPath, Json(exception.Actual));

            await DiffRunner.LaunchAsync(expectedPath, actualPath);
        }
    }
}
