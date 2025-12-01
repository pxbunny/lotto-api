using Azure.Data.Tables;
using Lotto.Storage;

namespace Lotto.Features.Development.DropDrawResultsTable;

internal sealed class DrawResultsRepository(TableServiceClient tableServiceClient, IOptions<TableOptions> tableOptions)
{
    private readonly string _tableName = tableOptions.Value.DrawResultsTableName;

    public async Task DropTableAsync(CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(_tableName);
        await client.DeleteAsync(cancellationToken);
    }
}
