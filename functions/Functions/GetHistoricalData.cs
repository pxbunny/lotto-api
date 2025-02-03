using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LottoDrawHistory.Functions;

internal sealed class DrawResultsEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "";

    public string RowKey { get; set; } = "";
    
    public DateTimeOffset? Timestamp { get; set; }
    
    public ETag ETag { get; set; }

    public string DrawDate { get; set; } = "";

    public string LottoNumbers { get; set; } = "";
    
    public string? PlusNumbers { get; set; }
}

internal sealed record DrawResultsDto(string DrawDate, IEnumerable<int> LottoNumbers, IEnumerable<int> PlusNumbers);

internal sealed class GetHistoricalData(
    TableServiceClient tableServiceClient,
    ILogger<GetHistoricalData> logger)
{
    private const string Route = "historical-draw-results";
    
    [Function(nameof(GetHistoricalData))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)] HttpRequest req)
    {
        var (isQueryStringValid, errorMessage) = ValidateQueryString(req.Query);

        if (!isQueryStringValid)
        {
            logger.LogError(errorMessage); // TODO: fix logging
            return new BadRequestObjectResult(errorMessage);
        }
        
        var (dateFrom, dateTo, limit) = ParseQueryString(req.Query);

        const string tableName = "LottoResults";
        const int maxPageSize = 1_000;
        
        var tableClient = tableServiceClient.GetTableClient(tableName);
        var filter = $"PartitionKey eq 'LottoData'";
        var query = tableClient.QueryAsync<DrawResultsEntity>(filter);
        var pageSize = limit > maxPageSize ? maxPageSize : limit;
        
        var results = new List<DrawResultsEntity>();

        await foreach (var page in query.AsPages(pageSizeHint: pageSize))
        {
            var remaining = limit - results.Count;
            var values = remaining < page.Values.Count
                ? page.Values.Take(remaining)
                : page.Values;
            
            results.AddRange(values);
            
            if (results.Count >= limit)
            {
                break;
            }
        }

        var dto = results.Select(r => new DrawResultsDto(
            r.DrawDate,
            r.LottoNumbers.Split(',').Select(int.Parse),
            r.PlusNumbers?.Split(',').Select(int.Parse) ?? []));

        // logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(dto);
    }
    
    private static (bool IsValid, string? ErrorMessage) ValidateQueryString(IQueryCollection query)
    {
        if (query.TryGetValue("dateFrom", out var dateFromStr) &&
            !string.IsNullOrWhiteSpace(dateFromStr) &&
            !DateOnly.TryParseExact(dateFromStr, "yyyy-M-d", out _))
        {
            return (false, "'dateFrom' must be a valid date in the format YYYY-M-D.");
        }

        if (query.TryGetValue("dateTo", out var dateToStr) &&
            !string.IsNullOrWhiteSpace(dateToStr) &&
            !DateOnly.TryParseExact(dateToStr, "yyyy-M-d", out _))
        {
            return (false, "'dateTo' must be a valid date in the format YYYY-M-D.");
        }

        if (query.TryGetValue("limit", out var limitStr) &&
            !string.IsNullOrWhiteSpace(limitStr) &&
            (!int.TryParse(limitStr, out var limit) || limit <= 0))
        {
            return (false, "'limit' must be a positive integer.");
        }

        return (true, null);
    }

    private static (DateOnly?, DateOnly?, int) ParseQueryString(IQueryCollection query)
    {
        DateOnly? dateFrom = null;
        DateOnly? dateTo = null;
        var limit = 100;

        if (query.TryGetValue("dateFrom", out var dateFromStr) &&
            DateOnly.TryParse(dateFromStr, out var parsedDateFrom))
        {
            dateFrom = parsedDateFrom;
        }

        if (query.TryGetValue("dateTo", out var dateToStr) &&
            DateOnly.TryParse(dateToStr, out var parsedDateTo))
        {
            dateTo = parsedDateTo;
        }

        if (query.TryGetValue("limit", out var limitStr) &&
            int.TryParse(limitStr, out var parsedLimit))
        {
            limit = parsedLimit;
        }

        return (dateFrom, dateTo, limit);
    }
}
