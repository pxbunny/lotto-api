using Microsoft.AspNetCore.Http;

namespace LottoDrawHistory.Functions.Http.Shared;

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

        if (request.Query.TryGetValue("top", out var topStr) &&
            !string.IsNullOrWhiteSpace(topStr) &&
            (!int.TryParse(topStr, out var top) || top <= 0))
        {
            return (false, "'top' must be a positive integer.");
        }

        return (true, null);
    }
    
    public static (DateOnly?, DateOnly?, int?) ParseQueryString(this HttpRequest req)
    {
        DateOnly? dateFrom = null;
        DateOnly? dateTo = null;
        int? top = null;

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

        if (req.Query.TryGetValue("top", out var topStr) &&
            int.TryParse(topStr, out var parsedTopValue))
        {
            top = parsedTopValue;
        }

        return (dateFrom, dateTo, top);
    }

    private static bool ValidateDateQueryStringValue(IQueryCollection query, string name) =>
        query.TryGetValue(name, out var dateStr) &&
        !string.IsNullOrWhiteSpace(dateStr) &&
        !DateOnly.TryParseExact(dateStr, Constants.DateFormat, out _);
}
