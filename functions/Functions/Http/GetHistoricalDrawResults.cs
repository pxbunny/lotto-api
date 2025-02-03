using JetBrains.Annotations;
using LottoDrawHistory.CQRS;
using LottoDrawHistory.Functions.Http.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions.Http;

sealed class GetHistoricalDrawResults(
    IMediator mediator,
    ILogger<GetHistoricalDrawResults> logger)
{
    [Function(nameof(GetHistoricalDrawResults))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "historical-draw-results")] HttpRequest req)
    {
        logger.LogInformation("Received request for historical draw results. Parameters : {QueryString}", req.QueryString);
        
        var (isValid, errorMessage) = req.ValidateQueryString();

        if (!isValid)
        {
            logger.LogError("Query string validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new { error = errorMessage });
        }
        
        var (dateFrom, dateTo, limit) = req.ParseQueryString();
        
        var query = new GetHistoricalDrawResultsQuery(dateFrom, dateTo, limit);
        var response = (await mediator.Send(query)).ToList();
        
        if (response.Count == 0)
        {
            logger.LogWarning("No results found for the given query parameters.");
            return new NotFoundObjectResult("No historical draw results found.");
        }
        
        var dto = response.Select(r => new DrawResultsDto(r.DrawDate, r.LottoNumbers, r.PlusNumbers));
        logger.LogInformation("Successfully retrieved {ResultCount} results. Response code 200 (OK).", dto.Count());
        return new OkObjectResult(dto);
    }
}

[UsedImplicitly]
sealed record DrawResultsDto(string DrawDate, IEnumerable<int> LottoNumbers, IEnumerable<int> PlusNumbers);
