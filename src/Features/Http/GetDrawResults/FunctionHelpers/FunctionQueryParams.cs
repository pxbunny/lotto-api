using System.Globalization;

namespace Lotto.Features.Http.GetDrawResults.FunctionHelpers;

internal sealed record FunctionQueryParams(DateOnly? DateFrom, DateOnly? DateTo, int? Top)
{
    public static FunctionQueryParams Parse(IQueryCollection query, out string? errorMessage)
    {
        DateOnly? dateFrom = null;
        DateOnly? dateTo = null;
        int? top = null;

        if (TryParseDate(query, "dateFrom", out var parsedDateFrom))
            dateFrom = parsedDateFrom;

        if (TryParseDate(query, "dateTo", out var parsedDateTo))
            dateTo = parsedDateTo;

        if (query.TryGetValue("top", out var topStr) && int.TryParse(topStr, out var parsedTopValue))
            top = parsedTopValue;

        var queryParams = new FunctionQueryParams(dateFrom, dateTo, top);
        errorMessage = ValidateQueryParams(query, queryParams);

        return queryParams;
    }

    private static string? ValidateQueryParams(IQueryCollection query, FunctionQueryParams queryParams)
    {
#pragma warning disable IDE0046
        if (query.TryGetValue("dateFrom", out _) && queryParams.DateFrom is null)
            return $"'dateFrom' must be a valid date in the format {Defaults.DateFormat}.";

        if (query.TryGetValue("dateTo", out _) && queryParams.DateTo is null)
            return $"'dateTo' must be a valid date in the format {Defaults.DateFormat}.";

        if (query.TryGetValue("top", out var topStr) && (!int.TryParse(topStr, out var topValue) || topValue <= 0))
            return "'top' must be a positive integer.";

        return null;
#pragma warning restore IDE0046
    }

    private static bool TryParseDate(IQueryCollection query, string name, out DateOnly date)
    {
        date = default;
        return query.TryGetValue(name, out var value) &&
               DateOnly.TryParseExact(value,
                   Defaults.DateFormat,
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out date);
    }
}
