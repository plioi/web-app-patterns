using System.Text.RegularExpressions;

namespace ContactList.Client;

public static class StringExtensions
{
    public static string PascalCaseToWords(this string self)
        => Regex.Replace(self, "([A-Z])", " $1").Trim();
}
