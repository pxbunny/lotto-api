using System.Globalization;
using System.Text;
using CsvHelper;
using JetBrains.Annotations;
using LottoDrawHistory.CQRS;
using LottoDrawHistory.Functions.Http.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions.Http;

sealed class DownloadHistoricalDrawResultsCsv(
    IMediator mediator,
    ILogger<DownloadHistoricalDrawResultsCsv> logger)
{
    [Function(nameof(DownloadHistoricalDrawResultsCsv))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "historical-draw-results/download")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received request for CSV file download. Parameters : {QueryString}", req.QueryString);
        
        var (isValid, errorMessage) = req.ValidateQueryString();

        if (!isValid)
        {
            logger.LogError("Query string validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new { error = errorMessage });
        }
        
        var (dateFrom, dateTo, limit) = req.ParseQueryString();
        
        var query = new GetHistoricalDrawResultsQuery(dateFrom, dateTo, limit);
        var response = (await mediator.Send(query, cancellationToken)).ToList();
        
        if (response.Count == 0)
        {
            logger.LogWarning("No results found for the given query parameters.");
            return new NotFoundObjectResult("No historical draw results found.");
        }
        
        var records = response
            .Select(r => new DrawResultsCsvRecord(r.DrawDate, r.LottoNumbersString, r.PlusNumbersString))
            .ToList();
        
        logger.LogInformation("Successfully retrieved {ResultCount} results. Preparing a CSV file.", records.Count);

        await using var writer = new StringWriter();
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(records, cancellationToken);
        
        return new FileContentResult(Encoding.UTF8.GetBytes(writer.ToString()), "application/octet-stream")
        {
            FileDownloadName = $"lotto-export_{DateTime.Now:yyyyMMddHHmmss}.csv"
        };
    }
}

[UsedImplicitly]
sealed record DrawResultsCsvRecord(string DrawDate, string LottoNumbers, string? PlusNumbers);
