using System.Globalization;
using Lotto.Storage.Entities;

namespace Lotto.Features.Http.GetDrawResults;

internal sealed record Request(DateOnly? DateFrom, DateOnly? DateTo, int? Top);

internal sealed class FunctionHandler(IDrawResultsRepository repository, ILogger<FunctionHandler> logger)
{
    public async Task<IEnumerable<DrawResults>> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var (dateFrom, dateTo, top) = request;

        logger.LogInformation(
            "Handling GetDrawResult - DateFrom: {DateFrom}, DateTo: {DateTo}, Top: {Top}",
            dateFrom, dateTo, top);

        var filter = BuildGetDrawResultsFilter(dateFrom, dateTo);

        logger.LogInformation("Final query filter: {Filter}", filter);
        logger.LogInformation("Fetching results from storage...");

        var resultsTopValue = top ?? int.MaxValue;

        var results = (await repository.GetAsync(filter, resultsTopValue, cancellationToken)).ToList();

        if (results.Count == 0)
        {
            logger.LogWarning("No draw results found for the given filter.");
            return [];
        }

        logger.LogInformation("Handled GetDrawResult. Successfully retrieved {Count} results.", results.Count);

        return results.Select(r => r.ToDrawResults());
    }

    private static string BuildGetDrawResultsFilter(DateOnly? dateFrom, DateOnly? dateTo)
    {
        var filter = "";

        if (dateFrom is not null)
            filter = $"DrawDate ge '{((DateOnly)dateFrom).ToString(Defaults.DateFormat, CultureInfo.InvariantCulture)}'";

        if (dateTo is not null)
            filter = $"{filter} and DrawDate le '{((DateOnly)dateTo).ToString(Defaults.DateFormat, CultureInfo.InvariantCulture)}'";

        if (filter.StartsWith(" and", StringComparison.InvariantCulture))
            filter = filter[4..];

        return filter;
    }
}
