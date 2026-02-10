using System.Net;
using System.Text.Json;
using Lotto.Storage.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Lotto.Features.Http.GetLatestDrawResults;

internal sealed class HttpGetFunction(
    IDrawResultsRepository repository,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger<HttpGetFunction> logger)
{
    private const string FunctionName = "GetLatestDrawResults";

    [Function(FunctionName)]
    [OpenApiOperation(FunctionName, "Draw Results")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(DrawResultsDto))]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(string))]
    [FunctionKeySecurity]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "draw-results/latest")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("{FunctionName} handling request.", FunctionName);
            var response = await HandleRequestAsync(cancellationToken);
            logger.LogInformation("{FunctionName} finished successfully.", FunctionName);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, "{FunctionName} failed while processing request.", FunctionName);
            throw;
        }
    }

    private async Task<IActionResult> HandleRequestAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching draw results...");

        var result = (await repository.GetLatestAsync(cancellationToken)).ToDrawResults();

        var dto = new DrawResultsDto(
            DrawDate: result.DrawDate,
            LottoNumbers: result.LottoNumbers,
            PlusNumbers: result.PlusNumbers);

        return new ContentResult
        {
            Content = JsonSerializer.Serialize(dto, jsonSerializerOptions),
            ContentType = "application/json",
            StatusCode = 200
        };
    }
}
