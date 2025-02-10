using Azure.Data.Tables;

namespace LottoDrawHistory.Data;

sealed class DrawResultsService(TableServiceClient tableServiceClient)
{
    private const string TableName = "LottoResults";
    private const string PartitionKey = "LottoData";
    private const string BaseFilter = $"PartitionKey eq '{PartitionKey}'";
    private const int MaxPageSize = 1_000;

    private readonly TableClient _client = tableServiceClient.GetTableClient(TableName);

    public async Task AddAsync(DrawResults data, CancellationToken cancellationToken)
    {
        var drawDate = DateTime.Parse(data.DrawDate);
        var dateDifference = DateTime.MaxValue - drawDate;
        var reversedDrawDate = DateTime.MinValue + dateDifference;
        var rowKey = reversedDrawDate.ToString(Constants.DateFormat.Replace("-", ""));

        var entity = new DrawResultsEntity
        {
            PartitionKey = PartitionKey,
            RowKey = rowKey,
            DrawDate = data.DrawDate,
            LottoNumbers = data.LottoNumbersString,
            PlusNumbers = data.PlusNumbersString
        };

        await _client.AddEntityAsync(entity, cancellationToken);
    }

    public async Task<DrawResultsEntity> GetLatestAsync(CancellationToken cancellationToken)
    {
        return (await GetAsync("", 1, cancellationToken)).FirstOrDefault()
               ?? throw new InvalidOperationException("Couldn't retrieve the latest draw results.");
    }

    public async Task<IEnumerable<DrawResultsEntity>> GetAsync(string filter, int top, CancellationToken cancellationToken)
    {
        var results = new List<DrawResultsEntity>();

        var fullFilter = !string.IsNullOrWhiteSpace(filter) ? $"{BaseFilter} and ({filter})" : BaseFilter;
        var query = _client.QueryAsync<DrawResultsEntity>(fullFilter, cancellationToken: cancellationToken);
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
