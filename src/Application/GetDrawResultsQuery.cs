using Lotto.Data;
using Lotto.Models;

namespace Lotto.Application;

internal sealed record GetDrawResultsQuery(
    DateOnly? DateFrom,
    DateOnly? DateTo,
    int? Top)
    : IRequest<IEnumerable<DrawResults>>;

internal sealed class GetHistoricalDrawResultsQueryHandler(
    DrawResultsService drawResultsService,
    ILogger<GetHistoricalDrawResultsQueryHandler> logger)
    : IRequestHandler<GetDrawResultsQuery, IEnumerable<DrawResults>>
{
    public async Task<IEnumerable<DrawResults>> Handle(
        GetDrawResultsQuery request,
        CancellationToken cancellationToken)
    {
        var (dateFrom, dateTo, top) = request;

        logger.LogInformation(
            "Handling GetHistoricalDrawResultsQuery - DateFrom: {DateFrom}, DateTo: {DateTo}, Top: {Top}",
            dateFrom, dateTo, top);

        var filter = CreateFilter(dateFrom, dateTo);

        logger.LogInformation("Final query filter: {Filter}", filter);
        logger.LogInformation("Fetching results from DrawResultsService...");

        var resultsTopValue = top ?? int.MaxValue;

        var results = (await drawResultsService.GetAsync(filter, resultsTopValue, cancellationToken)).ToList();

        if (results.Count == 0)
        {
            logger.LogWarning("No historical draw results found for the given filter.");
            return [];
        }

        logger.LogInformation("Handled GetHistoricalDrawResultsQuery. Successfully retrieved {Count} results.", results.Count);

        return results.Select(r => new DrawResults
        {
            DrawDate = r.DrawDate,
            LottoNumbers = r.LottoNumbers.Split(',').Select(int.Parse),
            PlusNumbers = !string.IsNullOrWhiteSpace(r.PlusNumbers) ? r.PlusNumbers.Split(',').Select(int.Parse) : [],
            LottoNumbersString = r.LottoNumbers,
            PlusNumbersString = r.PlusNumbers
        });
    }

    private static string CreateFilter(DateOnly? dateFrom, DateOnly? dateTo)
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
