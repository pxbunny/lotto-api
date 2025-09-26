using System.Globalization;
using System.Text;
using System.Text.Json;
using CsvHelper;
using LottoDrawHistory.Functions.Http.GetDrawResults.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LottoDrawHistory.Functions.Http.GetDrawResults;

[UsedImplicitly]
internal sealed record DrawResultsDto(string DrawDate, IEnumerable<int> LottoNumbers, IEnumerable<int> PlusNumbers);

[UsedImplicitly]
internal sealed record DrawResultsCsvRecord(string DrawDate, string LottoNumbers, string? PlusNumbers);

internal sealed class GetDrawResultsFunction(
    IMediator mediator,
    IContentNegotiator<ContentType> contentNegotiator,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger<GetDrawResultsFunction> logger)
{
    private const string FunctionName = nameof(GetDrawResultsFunction);

    [Function(nameof(GetDrawResultsFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "draw-results")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {FunctionName}. Parameters : {QueryString}", FunctionName, req.QueryString);

        try
        {
            var response = await HandleAsync(req, cancellationToken);
            logger.LogInformation("{FunctionName} finished successfully.", FunctionName);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError("{FunctionName} Failed. Error: {ErrorMessage}", FunctionName, e.Message);
            throw;
        }
    }

    private async Task<IActionResult> HandleAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        var (isValid, errorMessage) = request.ValidateQueryString();

        if (!isValid)
        {
            logger.LogError("Parameters validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new { error = errorMessage });
        }

        var (negotiationResult, contentType) = contentNegotiator.Negotiate(request);

        if (!negotiationResult)
            return HandleUnsupportedContentType(request);

        var query = request.ParseQueryString();
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

    private BadRequestObjectResult HandleUnsupportedContentType(HttpRequest request)
    {
        var acceptHeader = request.Headers.Accept;
        logger.LogError("Unsupported 'Accept' header value: {AcceptHeader}", acceptHeader!);
        return new BadRequestObjectResult(new { error = $"Unsupported 'Accept' header value: {acceptHeader}" });
    }
}
