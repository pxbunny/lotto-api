using System.Globalization;

namespace LottoDrawHistory.Functions.Http.GetDrawResults.Extensions;

internal static class HeadersExtensions
{
    public static ContentType? GetAcceptHeader(this HttpRequest req)
    {
        var acceptHeader = req.Headers.Accept.ToString();

        if (string.IsNullOrWhiteSpace(acceptHeader))
            return null;

        var mediaTypeEntries = acceptHeader.Split(',')
            .Select(entry =>
            {
                var parts = entry.Trim().Split(';').Select(p => p.Trim()).ToList();
                var mediaType = parts[0].ToLowerInvariant();
                var q = 1.0;

                foreach (var param in parts.Skip(1))
                {
                    if (!param.StartsWith("q=", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var qValue = param[2..];

                    if (double.TryParse(qValue,
                                        NumberStyles.AllowDecimalPoint,
                                        CultureInfo.InvariantCulture,
                                        out var parsedQ))
                        q = parsedQ;
                }

                return new { MediaType = mediaType, Q = q };
            })
            .OrderByDescending(x => x.Q);

        foreach (var entry in mediaTypeEntries)
        {
            switch (entry.MediaType)
            {
                case "application/json":
                    return ContentType.ApplicationJson;
                case "application/octet-stream":
                    return ContentType.ApplicationOctetStream;
                case "*/*":
                case "application/*":
                    return ContentType.ApplicationJson;
            }
        }

        return null;
    }
}
