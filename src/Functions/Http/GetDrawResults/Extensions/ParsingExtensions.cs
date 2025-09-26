using System.Globalization;
using LottoDrawHistory.Application;

namespace LottoDrawHistory.Functions.Http.GetDrawResults.Extensions;

internal static class ParsingExtensions
{
    public static GetHistoricalDrawResultsQuery ParseQueryString(this HttpRequest req)
    {
        DateOnly? dateFrom = null;
        DateOnly? dateTo = null;
        int? top = null;

        if (req.Query.TryParseDate("dateFrom", out var parsedDateFrom))
            dateFrom = parsedDateFrom;

        if (req.Query.TryParseDate("dateTo", out var parsedDateTo))
            dateTo = parsedDateTo;

        if (req.Query.TryGetValue("top", out var topStr) && int.TryParse(topStr, out var parsedTopValue))
            top = parsedTopValue;

        return new GetHistoricalDrawResultsQuery(dateFrom, dateTo, top);
    }

    private static bool TryParseDate(this IQueryCollection query, string name, out DateOnly date)
    {
        date = default;
        return query.TryGetValue(name, out var dateFrom) &&
               DateOnly.TryParseExact(dateFrom,
                   Constants.DateFormat,
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out date);
    }
}
