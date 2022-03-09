using static System.Environment;

namespace ContactList.Server.Tests;

class MatchException : Exception
{
    public object? Expected { get; }
    public object? Actual { get; }

    public MatchException(object? expected, object? actual)
        : base(
            $"Expected two objects to match on all properties.{NewLine}{NewLine}" +
            $"Expected:{NewLine}{Json(expected)}{NewLine}{NewLine}" +
            $"Actual:{NewLine}{Json(actual)}")
    {
        Expected = expected;
        Actual = actual;
    }
}
