using Lotto.Features.GetDrawResults.FunctionHelpers;
using Microsoft.AspNetCore.Mvc;

namespace Lotto.Features.GetDrawResults;

sealed class HttpGetFunction(
    IHandler<FunctionHandler, Request, IEnumerable<DrawResults>> handler,
    IFunctionResponseHandler responseHandler,
    IContentNegotiator<ContentType> contentNegotiator,
    ILogger<HttpGetFunction> logger)
{
    const string FunctionName = "GetDrawResults";

    [Function(FunctionName)]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "draw-results")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {FunctionName}. Parameters : {QueryString}", FunctionName, request.QueryString);

        try
        {
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

    async Task<IActionResult> HandleRequestAsync(HttpRequest request, CancellationToken cancellationToken)
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

    BadRequestObjectResult HandleUnsupportedContentType(HttpRequest request)
    {
        var acceptHeader = request.Headers.Accept;
        logger.LogError("Unsupported 'Accept' header value: {AcceptHeader}", acceptHeader!);
        return new BadRequestObjectResult(new { error = $"Unsupported 'Accept' header value: {acceptHeader}" });
    }
}
