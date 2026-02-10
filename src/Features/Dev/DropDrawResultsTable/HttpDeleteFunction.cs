using System.Net;
using Azure.Data.Tables;
using Lotto.Features.Http;
using Lotto.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Lotto.Features.Dev.DropDrawResultsTable;

internal sealed class HttpDeleteFunction(
    TableServiceClient tableServiceClient,
    IOptions<TableOptions> tableOptions,
    ILogger<HttpDeleteFunction> logger)
{
    private const string FunctionName = "DropDrawResultsTable";

    [Function(FunctionName)]
    [Operation(FunctionName, "Development")]
    [NoBodyResponse(HttpStatusCode.OK)]
    [FunctionKeySecurity]
    public async Task<IActionResult> Run(
        [HttpTrigger("delete", Route = "draw-results-table")] HttpRequest _,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("{FunctionName} triggered.", FunctionName);
        var tableName = tableOptions.Value.DrawResultsTableName;
        var client = tableServiceClient.GetTableClient(tableName);
        await client.DeleteAsync(cancellationToken);
        logger.LogInformation("{FunctionName} finished.", FunctionName);
        return new OkResult();
    }
}
