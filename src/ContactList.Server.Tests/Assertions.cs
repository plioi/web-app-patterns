namespace ContactList.Server.Tests;

static class Assertions
{
    public static void ShouldMatch<T>(this IEnumerable<T> actual, params T[] expected)
        => actual.ToArray().ShouldMatch(expected);

    public static void ShouldMatch<T>(this T actual, T expected)
    {
        if (Json(expected) != Json(actual))
            throw new MatchException(expected, actual);
    }
}
