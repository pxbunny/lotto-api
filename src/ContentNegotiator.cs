using Microsoft.Net.Http.Headers;

namespace Lotto;

internal interface IContentNegotiator<T> where T : struct, Enum
{
    NegotiationResult<T> Negotiate(HttpRequest request);
}

internal class ContentNegotiator<T> : IContentNegotiator<T>  where T : struct, Enum
{
    private readonly Dictionary<string, T> _configuration = new(StringComparer.OrdinalIgnoreCase);

    public ContentNegotiator(Action<IDictionary<string, T>> config) => config(_configuration);

    public NegotiationResult<T> Negotiate(HttpRequest request)
    {
        var acceptHeader = request.Headers.Accept.ToString();
        var supportedContentTypes = _configuration.Keys.Distinct();

        if (TryParseAcceptHeaderValues(acceptHeader, out var acceptHeaderValues) || acceptHeaderValues is null)
            return NegotiationResult<T>.Unsupported();

        var bestAcceptHeader = FindBestAcceptHeader(acceptHeaderValues, supportedContentTypes);

        return string.IsNullOrWhiteSpace(bestAcceptHeader)
            ? NegotiationResult<T>.Unsupported()
            : _configuration[bestAcceptHeader];
    }

    private static bool TryParseAcceptHeaderValues(string header, out IList<MediaTypeHeaderValue>? values)
    {
        values = null;

        return string.IsNullOrWhiteSpace(header) ||
               !MediaTypeHeaderValue.TryParseList(header.Split(','), out values) ||
               values.Count == 0;
    }

    private static string? FindBestAcceptHeader(
        IEnumerable<MediaTypeHeaderValue> acceptHeaderValues,
        IEnumerable<string> supportedContentTypes)
    {
        var bestValue = acceptHeaderValues
            .OrderByDescending(v => v.Quality ?? 1.0)
            .Select(v => v.MediaType.Value)
            .FirstOrDefault(mt => supportedContentTypes.Contains(mt, StringComparer.OrdinalIgnoreCase));

        return bestValue;
    }
}

internal record NegotiationResult<T>(bool Success, T ContentType) where T : struct, Enum
{
    public static NegotiationResult<T> Unsupported() => new(false, default);

    public static implicit operator NegotiationResult<T>(T contentType) => new(true, contentType);
}
