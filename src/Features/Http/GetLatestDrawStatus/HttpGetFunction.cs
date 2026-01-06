using System.Net;
using System.Net.Http;
using System.Text.Json;
using Lotto.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Lotto.Features.Http.GetLatestDrawStatus;

internal sealed class HttpGetFunction(
    FunctionHandler handler,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger<HttpGetFunction> logger)
{
    private const string FunctionName = "GetLatestDrawStatus";

    [Function(FunctionName)]
    [OpenApiOperation(FunctionName, "Draw Results")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(LatestDrawStatusDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadGateway, "application/json", typeof(ErrorResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, "application/json", typeof(ErrorResponse))]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "draw/status")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("{FunctionName} handling request.", FunctionName);
            var status = await handler.HandleAsync(cancellationToken);
            logger.LogInformation("{FunctionName} finished successfully.", FunctionName);

            return new ContentResult
            {
                Content = JsonSerializer.Serialize(status, jsonSerializerOptions),
                ContentType = "application/json",
                StatusCode = 200
            };
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, "{FunctionName} failed to fetch data from API.", FunctionName);

            return new ObjectResult(new ErrorResponse("Failed to retrieve data from external API."))
            {
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}
