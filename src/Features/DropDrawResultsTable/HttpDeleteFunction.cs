using Microsoft.AspNetCore.Mvc;

namespace Lotto.Features.DropDrawResultsTable;

internal sealed class HttpDeleteFunction(IDrawResultsRepository repository, ILogger<HttpDeleteFunction> logger)
{
    private const string FunctionName = "DropDrawResultsTable";

    [Function(FunctionName)]
    public async Task<IActionResult> Run(
        [HttpTrigger("delete", Route = "draw-results-table")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"{FunctionName} triggered.");
        await repository.DropTableAsync(cancellationToken);
        return new OkResult();
    }
}
