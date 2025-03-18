using LottoDrawHistory.Application;
using Microsoft.AspNetCore.Http;

namespace LottoDrawHistory.Functions.Http.Parsing;

static class HttpRequestExtensions
{
    public static GetHistoricalDrawResultsQuery ParseQueryString(this HttpRequest req)
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

        return new GetHistoricalDrawResultsQuery(dateFrom, dateTo, top);
    }
}
