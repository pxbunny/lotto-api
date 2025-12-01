using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Lotto.Features.Development.DropDrawResultsTable;

internal sealed class HttpDeleteFunction(DrawResultsRepository repository, ILogger<HttpDeleteFunction> logger)
{
    private const string FunctionName = "DropDrawResultsTable";

    [Function(FunctionName)]
    [OpenApiOperation(FunctionName, "Development")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK)]
    public async Task<IActionResult> Run(
        [HttpTrigger("delete", Route = "draw-results-table")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("{FunctionName} triggered.", FunctionName);
        await repository.DropTableAsync(cancellationToken);
        logger.LogInformation("{FunctionName} finished.", FunctionName);
        return new OkResult();
    }
}
