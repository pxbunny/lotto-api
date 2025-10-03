using Microsoft.AspNetCore.Mvc;

namespace Lotto.Functions.Http.GetDrawResults;

internal sealed class GetDrawResultsFunction(
    IDrawResultsService drawResultsService,
    IContentNegotiator<ContentType> contentNegotiator,
    IGetDrawResultsResponseHandler responseHandler,
    ILogger<GetDrawResultsFunction> logger)
{
    private const string FunctionName = nameof(GetDrawResultsFunction);

    [Function(nameof(GetDrawResultsFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "draw-results")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {FunctionName}. Parameters : {QueryString}", FunctionName, request.QueryString);

        try
        {
            var response = await HandleAsync(request, cancellationToken);
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
        var queryParams = GetDrawResultsQueryParams.Parse(request.Query, out var errorMessage);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            logger.LogError("Query parameters validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new { error = errorMessage });
        }

        var (negotiationResult, contentType) = contentNegotiator.Negotiate(request);

        if (!negotiationResult)
            return HandleUnsupportedContentType(request);

        var (dateFrom, dateTo, top) = queryParams;
        var results = (await drawResultsService.GetDrawResultsAsync(dateFrom, dateTo, top, cancellationToken)).ToList();
        return await responseHandler.HandleAsync(results, contentType, cancellationToken);
    }

    private BadRequestObjectResult HandleUnsupportedContentType(HttpRequest request)
    {
        var acceptHeader = request.Headers.Accept;
        logger.LogError("Unsupported 'Accept' header value: {AcceptHeader}", acceptHeader!);
        return new BadRequestObjectResult(new { error = $"Unsupported 'Accept' header value: {acceptHeader}" });
    }
}
