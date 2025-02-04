using JetBrains.Annotations;
using LottoDrawHistory.Functions.Http.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions.Http;

sealed class GetHistoricalDrawResults(HttpRequestHandler<GetHistoricalDrawResults> handler)
{
    [Function(nameof(GetHistoricalDrawResults))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "historical-draw-results")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        return await handler.HandleAsync(req, CreateResponse, cancellationToken);
    }

    private static IActionResult CreateResponse(IList<DrawResults> data, ILogger<GetHistoricalDrawResults> logger)
    {
        var dto = data.Select(r => new DrawResultsDto(r.DrawDate, r.LottoNumbers, r.PlusNumbers));
        logger.LogInformation("Successfully retrieved {ResultCount} results. Sending JSON response...", dto.Count());
        return new OkObjectResult(dto);
    }
}

[UsedImplicitly]
sealed record DrawResultsDto(string DrawDate, IEnumerable<int> LottoNumbers, IEnumerable<int> PlusNumbers);
