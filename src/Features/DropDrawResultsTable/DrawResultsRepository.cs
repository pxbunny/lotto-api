using Azure.Data.Tables;

namespace Lotto.Features.DropDrawResultsTable;

internal interface IDrawResultsRepository
{
    Task DropTableAsync(CancellationToken cancellationToken);
}

internal sealed class DrawResultsRepository(TableServiceClient tableServiceClient) : IDrawResultsRepository
{
    public async Task DropTableAsync(CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.DeleteAsync(cancellationToken);
    }
}
