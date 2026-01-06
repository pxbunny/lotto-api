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
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(string))]
    [OpenApiResponseWithBody(HttpStatusCode.NotAcceptable, "application/json", typeof(ErrorResponse))]
    [OpenApiResponseWithBody((HttpStatusCode)999, "application/octet-stream", typeof(byte[]))]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "draw-results")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "{FunctionName} handling request with query {QueryString} and accept {AcceptHeader}",
                FunctionName, request.QueryString, request.Headers.Accept.ToString());
            var response = await HandleRequestAsync(request, cancellationToken);
            logger.LogInformation("{FunctionName} finished successfully.", FunctionName);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, "{FunctionName} failed while processing request.", FunctionName);
            throw;
        }
    }

    private async Task<IActionResult> HandleRequestAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        var queryParams = FunctionQueryParams.Parse(request.Query, out var errorMessage);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            logger.LogError("Query parameters validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new ErrorResponse(errorMessage));
        }

        var (negotiationResult, contentType) = contentNegotiator.Negotiate(request);

        if (!negotiationResult)
            return HandleUnsupportedContentType(request);

        var (dateFrom, dateTo, top) = queryParams;
        var results = (await handler.HandleAsync(new Request(dateFrom, dateTo, top), cancellationToken)).ToList();
        return await responseHandler.HandleAsync(results, contentType, cancellationToken);
    }

    private ObjectResult HandleUnsupportedContentType(HttpRequest request)
    {
        var acceptHeader = request.Headers.Accept;
        logger.LogError("Unsupported 'Accept' header value: {AcceptHeader}", acceptHeader!);
        return new ObjectResult(new ErrorResponse($"Unsupported 'Accept' header value: {acceptHeader}"))
        {
            StatusCode = StatusCodes.Status406NotAcceptable
        };
    }
}
