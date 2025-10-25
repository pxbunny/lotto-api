using Azure.Data.Tables;

namespace Lotto.Features.DropDrawResultsTable;

interface IDrawResultsRepository
{
    Task DropTableAsync(CancellationToken cancellationToken);
}

sealed class DrawResultsRepository(TableServiceClient tableServiceClient) : IDrawResultsRepository
{
    public async Task DropTableAsync(CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.DeleteAsync(cancellationToken);
    }
}
