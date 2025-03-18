using Microsoft.AspNetCore.Http;

namespace LottoDrawHistory.Functions.Http.Headers;

static class HttpRequestExtensions
{
    public static ContentType? GetContentType(this HttpRequest req)
    {
        var acceptHeader = req.Headers.Accept.ToString();

        if (string.IsNullOrWhiteSpace(acceptHeader))
            return null;

        if (acceptHeader.Contains("application/json"))
            return ContentType.ApplicationJson;

        if (acceptHeader.Contains("application/octet-stream"))
            return ContentType.ApplicationOctetStream;

        if (new[] { "*/*", "application/*" }.Any(x => acceptHeader.Contains(x)))
            return ContentType.ApplicationJson;

        return null;
    }
}
