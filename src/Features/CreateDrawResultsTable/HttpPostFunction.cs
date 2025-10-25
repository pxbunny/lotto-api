using Microsoft.AspNetCore.Mvc;

namespace Lotto.Features.CreateDrawResultsTable;

sealed class HttpPostFunction(IDrawResultsRepository drawResultsRepository, ILogger<HttpPostFunction> logger)
{
    const string FunctionName = "CreateDrawResultsTable";

    [Function(FunctionName)]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "draw-results-table")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"{FunctionName} function triggered.");
        await drawResultsRepository.CreateTableIfNotExistsAsync(cancellationToken);
        logger.LogInformation($"{FunctionName} function finished.");
        return new OkResult();
    }
}
