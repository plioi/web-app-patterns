using System.Text.Json;

namespace ContactList.Server.Tests;

static class Utilities
{
    public static string Json(object? value) =>
        JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });
}
