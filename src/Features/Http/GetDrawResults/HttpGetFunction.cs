using System.Net;
using Lotto.Features.Http.GetDrawResults.FunctionHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace Lotto.Features.Http.GetDrawResults;

internal sealed class HttpGetFunction(
    FunctionHandler handler,
    FunctionResponseHandler responseHandler,
    IContentNegotiator<ContentType> contentNegotiator,
    ILogger<HttpGetFunction> logger)
{
    private const string FunctionName = "GetDrawResults";

    [Function(FunctionName)]
    [OpenApiOperation(FunctionName, "Draw Results")]
    [OpenApiParameter("dateFrom", In = ParameterLocation.Query)]
    [OpenApiParameter("dateTo", In = ParameterLocation.Query)]
    [OpenApiParameter("top", In = ParameterLocation.Query, Type = typeof(int))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(DrawResultsDto[]))]
    [OpenApiResponseWithBody((HttpStatusCode)999, "application/octet-stream", typeof(byte[]))]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "draw-results")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling {FunctionName}. Parameters : {QueryString}", FunctionName, request.QueryString);
            var response = await HandleRequestAsync(request, cancellationToken);
            logger.LogInformation("{FunctionName} finished successfully.", FunctionName);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError("{FunctionName} Failed. Error: {ErrorMessage}", FunctionName, e.Message);
            throw;
        }
    }

    private async Task<IActionResult> HandleRequestAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        var queryParams = FunctionQueryParams.Parse(request.Query, out var errorMessage);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            logger.LogError("Query parameters validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new { error = errorMessage });
        }

        var (negotiationResult, contentType) = contentNegotiator.Negotiate(request);

        if (!negotiationResult)
            return HandleUnsupportedContentType(request);

        var (dateFrom, dateTo, top) = queryParams;
        var results = (await handler.HandleAsync(new Request(dateFrom, dateTo, top), cancellationToken)).ToList();
        return await responseHandler.HandleAsync(results, contentType, cancellationToken);
    }

    private BadRequestObjectResult HandleUnsupportedContentType(HttpRequest request)
    {
        var acceptHeader = request.Headers.Accept;
        logger.LogError("Unsupported 'Accept' header value: {AcceptHeader}", acceptHeader!);
        return new BadRequestObjectResult(new { error = $"Unsupported 'Accept' header value: {acceptHeader}" });
    }
}
