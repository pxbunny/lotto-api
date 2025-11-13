using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Lotto.Features.Development.CreateDrawResultsTable;

internal sealed class HttpPostFunction(DrawResultsRepository repository, ILogger<HttpPostFunction> logger)
{
    private const string FunctionName = "CreateDrawResultsTable";

    [Function(FunctionName)]
    [OpenApiOperation(FunctionName, "Development")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK)]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "draw-results-table")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"{FunctionName} function triggered.");
        await repository.CreateTableIfNotExistsAsync(cancellationToken);
        logger.LogInformation($"{FunctionName} function finished.");
        return new OkResult();
    }
}
