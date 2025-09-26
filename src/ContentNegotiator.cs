using Microsoft.Net.Http.Headers;

namespace LottoDrawHistory;

internal interface IContentNegotiator
{
    ContentType Negotiate(HttpRequest request);
}

internal class ContentNegotiator : IContentNegotiator
{
    private const ContentType DefaultContentType =  ContentType.ApplicationJson;

    private static readonly IEnumerable<string> SupportedContentTypes =
    [
        "*/*",
        "application/*",
        "application/json",
        "application/octet-stream"
    ];

    public ContentType Negotiate(HttpRequest request)
    {
        var acceptHeader = request.Headers.Accept.ToString();

        if (TryParseAcceptHeaderValues(acceptHeader, out var acceptHeaderValues) || acceptHeaderValues is null)
            return ContentType.Unsupported;

        var bestAcceptHeader = FindBestAcceptHeader(acceptHeaderValues);
        var acceptHeaderValue = bestAcceptHeader?.ToLower() switch
        {
            "*/*" => DefaultContentType,
            "application/*"  => ContentType.ApplicationJson,
            "application/json" => ContentType.ApplicationJson,
            "application/octet-stream" => ContentType.ApplicationOctetStream,
            _ => ContentType.Unsupported
        };

        return acceptHeaderValue;
    }

    private static bool TryParseAcceptHeaderValues(string header, out IList<MediaTypeHeaderValue>? values)
    {
        values = null;

        return string.IsNullOrWhiteSpace(header) ||
               !MediaTypeHeaderValue.TryParseList(header.Split(','), out values) ||
               values.Count == 0;
    }

    private static string? FindBestAcceptHeader(IEnumerable<MediaTypeHeaderValue> acceptHeaderValues)
    {
        var bestValue = acceptHeaderValues
            .OrderByDescending(v => v.Quality ?? 1.0)
            .Select(v => v.MediaType.Value)
            .FirstOrDefault(mt => SupportedContentTypes.Contains(mt, StringComparer.OrdinalIgnoreCase));

        return bestValue;
    }
}
