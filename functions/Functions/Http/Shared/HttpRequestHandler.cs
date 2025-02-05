using LottoDrawHistory.CQRS;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions.Http.Shared;

sealed class HttpRequestHandler<TFunction>(IMediator mediator, ILogger<TFunction> logger)
{
    public async Task<IActionResult> HandleAsync(
        HttpRequest req,
        Func<IList<DrawResults>, ILogger<TFunction>, IActionResult> createResponse,
        CancellationToken cancellationToken = default)
    {
        return await HandleAsync(req, (data, _, _) => Task.FromResult(createResponse(data, logger)), cancellationToken);
    }

    public async Task<IActionResult> HandleAsync(
        HttpRequest req,
        Func<IList<DrawResults>, ILogger<TFunction>, CancellationToken, Task<IActionResult>> createResponseAsync,
        CancellationToken cancellationToken = default)
    {
        var functionName = typeof(TFunction).Name;
        logger.LogInformation("Handling {FunctionName}. Parameters : {QueryString}", functionName, req.QueryString);
        var (isValid, errorMessage) = req.ValidateQueryString();

        if (!isValid)
        {
            logger.LogError("Parameters validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new { error = errorMessage });
        }

        var (dateFrom, dateTo, top) = req.ParseQueryString();

        var query = new GetHistoricalDrawResultsQuery(dateFrom, dateTo, top);
        var response = (await mediator.Send(query, cancellationToken)).ToList();

        if (response.Count > 0)
        {
            var httpResponse = await createResponseAsync(response, logger, cancellationToken);
            logger.LogInformation("{FunctionName} handled successfully.", functionName);
            return httpResponse;
        }

        logger.LogWarning("No results found for the given query parameters.");
        return new NotFoundObjectResult("No historical draw results found.");
    }
}
