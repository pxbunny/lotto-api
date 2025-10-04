using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace Lotto.Functions.Http.CreateDrawResultsTable;

public class CreateDrawResultsTableFunction(TableServiceClient tableServiceClient)
{
    [Function(nameof(CreateDrawResultsTableFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "draw-results-table")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.CreateIfNotExistsAsync(cancellationToken);
        return new OkResult();
    }
}
