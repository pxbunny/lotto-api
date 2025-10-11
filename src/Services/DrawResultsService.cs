namespace Lotto.Services;

internal sealed class DrawResultsService(
    IDrawResultsRepository drawResultsRepository,
    ILogger<DrawResultsService> logger) : IDrawResultsService
{
    public async Task<IEnumerable<DrawResults>> GetDrawResultsAsync(
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int? top,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling GetDrawResult - DateFrom: {DateFrom}, DateTo: {DateTo}, Top: {Top}",
            dateFrom, dateTo, top);

        var filter = BuildGetDrawResultsFilter(dateFrom, dateTo);

        logger.LogInformation("Final query filter: {Filter}", filter);
        logger.LogInformation("Fetching results from storage...");

        var resultsTopValue = top ?? int.MaxValue;

        var results = (await drawResultsRepository.GetAsync(filter, resultsTopValue, cancellationToken)).ToList();

        if (results.Count == 0)
        {
            logger.LogWarning("No draw results found for the given filter.");
            return [];
        }

        logger.LogInformation("Handled GetDrawResult. Successfully retrieved {Count} results.", results.Count);

        return results.Select(r => new DrawResults
        {
            DrawDate = r.DrawDate,
            LottoNumbers = r.LottoNumbers.Split(',').Select(int.Parse),
            PlusNumbers = !string.IsNullOrWhiteSpace(r.PlusNumbers) ? r.PlusNumbers.Split(',').Select(int.Parse) : [],
            LottoNumbersString = r.LottoNumbers,
            PlusNumbersString = r.PlusNumbers
        });
    }

    private static string BuildGetDrawResultsFilter(DateOnly? dateFrom, DateOnly? dateTo)
    {
        var filter = "";

        if (dateFrom is not null)
            filter = $"DrawDate ge '{((DateOnly)dateFrom).ToString(Constants.DateFormat)}'";

        if (dateTo is not null)
            filter = $"{filter} and DrawDate le '{((DateOnly)dateTo).ToString(Constants.DateFormat)}'";

        if (filter.StartsWith(" and"))
            filter = filter.Remove(0, 4);

        return filter;
    }
}
