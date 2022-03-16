using FluentValidation.Results;
using MediatR;
using static System.Environment;

namespace ContactList.Server.Tests;

static class Assertions
{
    public static TException Throws<TException>(this Action operation) where TException : Exception
        => Should.Throw<TException>(operation);

    public static async Task<TException> ThrowsAsync<TException>(this Func<Task> operation) where TException : Exception
        => await Should.ThrowAsync<TException>(operation);

    public static void ShouldMatch<T>(this IEnumerable<T> actual, params T[] expected)
        => actual.ToArray().ShouldMatch(expected);

    public static void ShouldMatch<T>(this T actual, T expected)
    {
        if (Json(expected) != Json(actual))
            throw new MatchException(expected, actual);
    }

    public static async Task ShouldValidateAsync<TResult>(this IRequest<TResult> message)
    {
        var result = await ValidationAsync(message);
        result.ShouldBeSuccessful();
    }

    public static async Task ShouldNotValidateAsync<TResult>(this IRequest<TResult> message, params string[] expectedErrors)
    {
        var result = await ValidationAsync(message);
        result.ShouldBeFailure(expectedErrors);
    }

    public static void ShouldBeSuccessful(this ValidationResult result)
    {
        var indentedErrorMessages = result
            .Errors
            .OrderBy(x => x.ErrorMessage)
            .Select(x => "    " + x.ErrorMessage)
            .ToArray();

        var actual = String.Join(NewLine, indentedErrorMessages);

        result.IsValid.ShouldBeTrue($"Expected no validation errors, but found {result.Errors.Count}:{NewLine}{actual}");
    }

    public static void ShouldBeFailure(this ValidationResult result, params string[] expectedErrors)
    {
        result.IsValid.ShouldBeFalse("Expected validation errors, but the message passed validation.");

        result.Errors
            .OrderBy(x => x.ErrorMessage)
            .Select(x => x.ErrorMessage)
            .ShouldMatch(expectedErrors.OrderBy(x => x).ToArray());
    }
}
