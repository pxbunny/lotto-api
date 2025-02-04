using System.Globalization;
using System.Text;
using CsvHelper;
using JetBrains.Annotations;
using LottoDrawHistory.Functions.Http.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions.Http;

sealed class DownloadHistoricalDrawResultsCsv(HttpRequestHandler<DownloadHistoricalDrawResultsCsv> handler)
{
    [Function(nameof(DownloadHistoricalDrawResultsCsv))]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "historical-draw-results/download")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        return await handler.HandleAsync(req, CreateResponseAsync, cancellationToken);
    }

    private static async Task<IActionResult> CreateResponseAsync(
        IList<DrawResults> data,
        ILogger<DownloadHistoricalDrawResultsCsv> logger,
        CancellationToken cancellationToken)
    {
        var records = data
            .Select(r => new DrawResultsCsvRecord(r.DrawDate, r.LottoNumbersString, r.PlusNumbersString))
            .ToList();

        logger.LogInformation("Successfully retrieved {ResultCount} results. Creating a CSV file...", records.Count);

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
