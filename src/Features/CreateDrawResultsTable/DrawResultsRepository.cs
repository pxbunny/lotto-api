using Azure.Data.Tables;

namespace Lotto.Features.CreateDrawResultsTable;

interface IDrawResultsRepository
{
    Task CreateTableIfNotExistsAsync(CancellationToken cancellationToken);
}

sealed class DrawResultsRepository(TableServiceClient tableServiceClient) : IDrawResultsRepository
{
    public async Task CreateTableIfNotExistsAsync(CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.CreateIfNotExistsAsync(cancellationToken);
    }
}
