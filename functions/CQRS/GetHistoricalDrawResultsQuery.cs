using LottoDrawHistory.Data;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.CQRS;

sealed record GetHistoricalDrawResultsQuery(
    DateOnly? DateFrom,
    DateOnly? DateTo,
    int? Limit)
    : IRequest<IEnumerable<DrawResults>>;

sealed class GetHistoricalDrawResultsQueryHandler(
    DrawResultsService drawResultsService,
    ILogger<GetHistoricalDrawResultsQueryHandler> logger)
    : IRequestHandler<GetHistoricalDrawResultsQuery, IEnumerable<DrawResults>>
{
    public async Task<IEnumerable<DrawResults>> Handle(
        GetHistoricalDrawResultsQuery request,
        CancellationToken cancellationToken)
    {
        var (dateFrom, dateTo, limit) = request;
        const int defaultLimit = 100;
        
        logger.LogInformation(
            "Handling GetHistoricalDrawResultsQuery - DateFrom: {DateFrom}, DateTo: {DateTo}, Limit: {Limit}",
            dateFrom, dateTo, limit);
        
        var filter = "";

        if (dateFrom is not null)
            filter = $"DrawDate ge '{((DateOnly)dateFrom).ToString(Constants.DateFormat)}'";

        if (dateTo is not null)
            filter = $"{filter} and DrawDate le '{((DateOnly)dateTo).ToString(Constants.DateFormat)}'";

        if (filter.StartsWith(" and"))
            filter = filter.Remove(0, 4);
        
        logger.LogInformation("Final query filter: {Filter}", filter);
        logger.LogInformation("Fetching results from DrawResultsService...");

        var resultsLimit = limit ?? (dateFrom is null && dateTo is null
            ? defaultLimit
            : int.MaxValue);

        var results = (await drawResultsService.GetAsync(filter, resultsLimit, cancellationToken)).ToList();
        
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
}
