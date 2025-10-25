using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;

namespace Lotto.Features.GetDrawResults.FunctionHelpers;

interface IFunctionResponseHandler
{
    Task<IActionResult> HandleAsync(
        IList<DrawResults> results,
        ContentType contentType,
        CancellationToken cancellationToken);
}

sealed class FunctionResponseHandler(
    JsonSerializerOptions jsonSerializerOptions,
    ILogger<FunctionResponseHandler> logger) : IFunctionResponseHandler
{
    public async Task<IActionResult> HandleAsync(
        IList<DrawResults> results,
        ContentType contentType,
        CancellationToken cancellationToken)
    {
        if (results.Count != 0)
        {
            return contentType switch
            {
                ContentType.ApplicationJson => CreateJsonResponse(results),
                ContentType.ApplicationOctetStream => await CreateCsvResponseAsync(results, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(contentType))
            };
        }

        logger.LogWarning("No results found for the given query parameters.");
        return new NotFoundObjectResult("No historical draw results found.");
    }

    ContentResult CreateJsonResponse(IList<DrawResults> data)
    {
        var dto = data.Select(r => new DrawResultsDto(r.DrawDate, r.LottoNumbers, r.PlusNumbers)).ToList();
        logger.LogInformation("Successfully retrieved {ResultCount} results. Sending JSON response...", dto.Count);

        return new ContentResult
        {
            Content = JsonSerializer.Serialize(dto, jsonSerializerOptions),
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    async Task<IActionResult> CreateCsvResponseAsync(
        IList<DrawResults> data,
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
