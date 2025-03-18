using Microsoft.AspNetCore.Http;

namespace LottoDrawHistory.Functions.Http.Validation;

static class HttpRequestExtensions
{
    public static (bool IsValid, string? ErrorMessage) ValidateQueryString(this HttpRequest request)
    {
        if (ValidateDateQueryStringValue(request.Query, "dateFrom"))
            return (false, $"'dateFrom' must be a valid date in the format {Constants.DateFormat}.");

        if (ValidateDateQueryStringValue(request.Query, "dateTo"))
            return (false, $"'dateTo' must be a valid date in the format {Constants.DateFormat}.");

        if (ValidateIntQueryStringValue(request.Query, "top"))
            return (false, "'top' must be a positive integer.");

        return (true, null);
    }

    private static bool ValidateDateQueryStringValue(IQueryCollection query, string name) =>
        query.TryGetValue(name, out var dateStr) &&
        !string.IsNullOrWhiteSpace(dateStr) &&
        !DateOnly.TryParseExact(dateStr, Constants.DateFormat, out _);

    private static bool ValidateIntQueryStringValue(IQueryCollection query, string name) =>
        query.TryGetValue(name, out var intStr) &&
        !string.IsNullOrWhiteSpace(intStr) &&
        (!int.TryParse(intStr, out var value) || value <= 0);
}
