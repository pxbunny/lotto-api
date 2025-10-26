using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Lotto.Features.DropDrawResultsTable;

sealed class HttpDeleteFunction(IDrawResultsRepository repository, ILogger<HttpDeleteFunction> logger)
{
    const string FunctionName = "DropDrawResultsTable";

    [Function(FunctionName)]
    [OpenApiOperation(FunctionName, "Dev Helpers")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK)]
    public async Task<IActionResult> Run(
        [HttpTrigger("delete", Route = "draw-results-table")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"{FunctionName} triggered.");
        await repository.DropTableAsync(cancellationToken);
        logger.LogInformation($"{FunctionName} function finished.");
        return new OkResult();
    }
}
