using Azure.Data.Tables;
using Lotto.Storage.Entities;

namespace Lotto.Features.GetDrawResults;

interface IDrawResultsRepository
{
    Task<IEnumerable<DrawResultsEntity>> GetAsync(string filter, int top, CancellationToken cancellationToken);
}

sealed class DrawResultsRepository(TableServiceClient tableServiceClient) : IDrawResultsRepository
{
    const string PartitionKey = "LottoData";
    const string BaseFilter = $"PartitionKey eq '{PartitionKey}'";
    const int MaxPageSize = 1_000;

    public async Task<IEnumerable<DrawResultsEntity>> GetAsync(string filter, int top, CancellationToken cancellationToken)
    {
        var results = new List<DrawResultsEntity>();

        var fullFilter = !string.IsNullOrWhiteSpace(filter) ? $"{BaseFilter} and ({filter})" : BaseFilter;
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        var query = client.QueryAsync<DrawResultsEntity>(fullFilter, cancellationToken: cancellationToken);
        var pageSize = Math.Min(MaxPageSize, top);

        await foreach (var page in query.AsPages(pageSizeHint: pageSize).WithCancellation(cancellationToken))
        {
            var remaining = top - results.Count;

            results.AddRange(
                remaining < page.Values.Count
                    ? page.Values.Take(remaining)
                    : page.Values);

            if (results.Count >= top) break;
        }

        return results;
    }
}
