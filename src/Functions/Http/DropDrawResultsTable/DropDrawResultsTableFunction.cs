using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace Lotto.Functions.Http.DropDrawResultsTable;

internal sealed class DropDrawResultsTableFunction(TableServiceClient tableServiceClient)
{
    [Function(nameof(DropDrawResultsTableFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger("delete", Route = "draw-results-table")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.DeleteAsync(cancellationToken);
        return new OkResult();
    }
}
