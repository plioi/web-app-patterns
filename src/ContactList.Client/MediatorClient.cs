using System.Net;
using System.Net.Http.Json;
using MediatR;

namespace ContactList.Client;

public class MediatorClient
{
    readonly HttpClient _http;
    readonly ILogger<MediatorClient> _logger;

    public MediatorClient(HttpClient http, ILogger<MediatorClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<TResponse> GetAsync<TResponse>(string route)
    {
        var typedResponse = await _http.GetFromJsonAsync<TResponse>(route);

        if (typedResponse == null)
            throw new Exception("HTTP response body was unexpectedly null.");

        return typedResponse;
    }

    public async Task<TResponse> GetAsync<TResponse>(string route, IRequest<TResponse> queryStringParameters)
    {
        var typedResponse = await _http.GetFromJsonAsync<TResponse>(AppendQueryString(route, queryStringParameters));

        if (typedResponse == null)
            throw new Exception("HTTP response body was unexpectedly null.");

        return typedResponse;
    }

    public async Task PostAsync<TRequest, TResponse>(
        string route, TRequest command,
        Func<TResponse, Task> onSuccess,
        Action<Dictionary<string, List<string>>> onServerValidationFailure)
        where TRequest : IRequest<TResponse>
    {
        try
        {
            var response = await _http.PostAsJsonAsync(route, command);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await ReadValidationMessagesAsync(response);

                if (errors.Count > 0)
                {
                    onServerValidationFailure(errors);
                    return;
                }
            }

            response.EnsureSuccessStatusCode();

            TResponse? typedResponse = default;

            if (typeof(TResponse) != typeof(Unit))
                typedResponse = await response.Content.ReadFromJsonAsync<TResponse>();

            if (typedResponse == null)
                throw new Exception("HTTP response body was unexpectedly null.");

            await onSuccess(typedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST request failed.");
            throw;
        }
    }

    static string AppendQueryString<TResponse>(string route, IRequest<TResponse> query)
    {
        var queryString = System.Web.HttpUtility.ParseQueryString("");

        var properties = query.GetType().GetProperties();

        foreach (var parameter in properties)
        {
            var value = parameter.GetValue(query);

            if (value != null)
                queryString.Add(parameter.Name, value.ToString());
        }

        var uri = route;

        if (queryString.Count > 0)
            uri += "?" + queryString;

        return uri;
    }

    static async Task<Dictionary<string, List<string>>> ReadValidationMessagesAsync(HttpResponseMessage response)
    {
        var parsedResponse = await response.Content.ReadFromJsonAsync<ValidationFailureResponse>();

        return parsedResponse?.Errors ?? new Dictionary<string, List<string>>();
    }

    class ValidationFailureResponse
    {
        public string? Title { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }
}
