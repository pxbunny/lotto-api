using System.Globalization;
using System.Text;
using System.Text.Json;
using CsvHelper;
using LottoDrawHistory.Functions.Http.Headers;
using LottoDrawHistory.Functions.Http.Parsing;
using LottoDrawHistory.Functions.Http.Validation;
using Microsoft.AspNetCore.Mvc;

namespace LottoDrawHistory.Functions.Http;

[UsedImplicitly]
sealed record DrawResultsDto(string DrawDate, IEnumerable<int> LottoNumbers, IEnumerable<int> PlusNumbers);

[UsedImplicitly]
sealed record DrawResultsCsvRecord(string DrawDate, string LottoNumbers, string? PlusNumbers);

sealed class GetHistoricalDrawResults(
    IMediator mediator,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger<GetHistoricalDrawResults> logger)
{
    private const string FunctionName = nameof(GetHistoricalDrawResults);

    [Function(nameof(GetHistoricalDrawResults))]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "historical-draw-results")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {FunctionName}. Parameters : {QueryString}", FunctionName, req.QueryString);
        var (isValid, errorMessage) = req.ValidateQueryString();

        if (!isValid)
        {
            logger.LogError("Parameters validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new { error = errorMessage });
        }

        var contentType = req.GetContentType();

        if (contentType is null)
        {
            var acceptHeader = req.Headers.Accept;
            logger.LogError("Invalid 'Accept' header value: {AcceptHeader}", acceptHeader!);
            return new BadRequestObjectResult(new { error = $"Invalid 'Accept' header value: {acceptHeader}" });
        }

        var query = req.ParseQueryString();
        var response = (await mediator.Send(query, cancellationToken)).ToList();

        if (response.Count == 0)
        {
            logger.LogWarning("No results found for the given query parameters.");
            return new NotFoundObjectResult("No historical draw results found.");
        }

        return contentType switch
        {
            ContentType.ApplicationJson => CreateJsonResponse(response),
            ContentType.ApplicationOctetStream => await CreateCsvResponseAsync(response, cancellationToken),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private ContentResult CreateJsonResponse(IList<DrawResults> data)
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

    private async Task<IActionResult> CreateCsvResponseAsync(IList<DrawResults> data, CancellationToken cancellationToken)
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
