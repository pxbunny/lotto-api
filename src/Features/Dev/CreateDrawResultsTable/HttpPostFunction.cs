using System.Net;
using Azure.Data.Tables;
using Lotto.Features.Http;
using Lotto.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Lotto.Features.Dev.CreateDrawResultsTable;

internal sealed class HttpPostFunction(
    TableServiceClient tableServiceClient,
    IOptions<TableOptions> tableOptions,
    ILogger<HttpPostFunction> logger)
{
    private const string FunctionName = "CreateDrawResultsTable";

    [Function(FunctionName)]
    [OpenApiOperation(FunctionName, "Development")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK)]
    [FunctionKeySecurity]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "draw-results-table")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("{FunctionName} triggered.", FunctionName);
        var tableName = tableOptions.Value.DrawResultsTableName;
        var client = tableServiceClient.GetTableClient(tableName);
        await client.CreateIfNotExistsAsync(cancellationToken);
        logger.LogInformation("{FunctionName} finished.", FunctionName);
        return new OkResult();
    }
}
