using Azure.Data.Tables;

namespace Lotto.Features.CreateDrawResultsTable;

internal interface IDrawResultsRepository
{
    Task CreateTableIfNotExistsAsync(CancellationToken cancellationToken);
}

internal sealed class DrawResultsRepository(TableServiceClient tableServiceClient) : IDrawResultsRepository
{
    public async Task CreateTableIfNotExistsAsync(CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.CreateIfNotExistsAsync(cancellationToken);
    }
}
