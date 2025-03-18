using Azure;
using Azure.Data.Tables;

namespace LottoDrawHistory.Data;

[UsedImplicitly]
sealed class DrawResultsEntity : ITableEntity
{
    public required string PartitionKey { get; set; }

    public required string RowKey { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public required string DrawDate { get; init; }

    public required string LottoNumbers { get; init; }

    public string? PlusNumbers { get; init; }
}
