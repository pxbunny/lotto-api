using Azure.Data.Tables;

namespace Lotto.Features.Development.DropDrawResultsTable;

internal sealed class DrawResultsRepository(TableServiceClient tableServiceClient)
{
    public async Task DropTableAsync(CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.DeleteAsync(cancellationToken);
    }
}
