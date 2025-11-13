using Azure.Data.Tables;

namespace Lotto.Features.Development.CreateDrawResultsTable;

internal sealed class DrawResultsRepository(TableServiceClient tableServiceClient)
{
    public async Task CreateTableIfNotExistsAsync(CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.CreateIfNotExistsAsync(cancellationToken);
    }
}
