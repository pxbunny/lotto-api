using Microsoft.AspNetCore.Http;

namespace LottoDrawHistory.Functions.Http.Extensions;

static class HttpRequestExtensions
{
    public static (bool IsValid, string? ErrorMessage) ValidateQueryString(this HttpRequest request)
    {
        if (ValidateDateQueryStringValue(request.Query, "dateFrom"))
        {
            return (false, $"'dateFrom' must be a valid date in the format {Constants.DateFormat}.");
        }

        if (ValidateDateQueryStringValue(request.Query, "dateTo"))
        {
            return (false, $"'dateTo' must be a valid date in the format {Constants.DateFormat}.");
        }

        if (request.Query.TryGetValue("limit", out var limitStr) &&
            !string.IsNullOrWhiteSpace(limitStr) &&
            (!int.TryParse(limitStr, out var limit) || limit <= 0))
        {
            return (false, "'limit' must be a positive integer.");
        }

        return (true, null);
    }
    
    public static (DateOnly?, DateOnly?, int) ParseQueryString(this HttpRequest req)
    {
        DateOnly? dateFrom = null;
        DateOnly? dateTo = null;
        var limit = 100;

        if (req.Query.TryGetValue("dateFrom", out var dateFromStr) &&
            DateOnly.TryParse(dateFromStr, out var parsedDateFrom))
        {
            dateFrom = parsedDateFrom;
        }

        if (req.Query.TryGetValue("dateTo", out var dateToStr) &&
            DateOnly.TryParse(dateToStr, out var parsedDateTo))
        {
            dateTo = parsedDateTo;
        }

        if (req.Query.TryGetValue("limit", out var limitStr) &&
            int.TryParse(limitStr, out var parsedLimit))
        {
            limit = parsedLimit;
        }

        return (dateFrom, dateTo, limit);
    }

    private static bool ValidateDateQueryStringValue(IQueryCollection query, string name) =>
        query.TryGetValue(name, out var dateStr) &&
        !string.IsNullOrWhiteSpace(dateStr) &&
        !DateOnly.TryParseExact(dateStr, Constants.DateFormat, out _);
}
