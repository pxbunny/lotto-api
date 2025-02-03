using Azure.Data.Tables;

namespace LottoDrawHistory.Data;

sealed class DrawResultsService(TableServiceClient tableServiceClient)
{
    private const string TableName = "LottoResults";
    private const int MaxPageSize = 1_000;

    private readonly TableClient _client = tableServiceClient.GetTableClient(TableName);

    public async Task<IEnumerable<DrawResultsEntity>> GetAsync(string filter, int limit, CancellationToken ct)
    {
        var query = _client.QueryAsync<DrawResultsEntity>(filter, cancellationToken: ct);
        var pageSize = limit > MaxPageSize ? MaxPageSize : limit;
        
        var results = new List<DrawResultsEntity>();

        await foreach (var page in query.AsPages(pageSizeHint: pageSize).WithCancellation(ct))
        {
            var remaining = limit - results.Count;
            var values = remaining < page.Values.Count
                ? page.Values.Take(remaining)
                : page.Values;
            
            results.AddRange(values);
            
            if (results.Count >= limit) break;
        }

        return results;
    }
}
