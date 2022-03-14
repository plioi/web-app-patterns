using System.Linq.Expressions;

namespace ContactList.Client;

public static class PropertyExpressionExtensions
{
    public static string DisplayName<T>(this Expression<Func<T>> accessor)
    {
        var expression = (MemberExpression)accessor.Body;

        return expression.Member.Name.PascalCaseToWords();
    }
}
