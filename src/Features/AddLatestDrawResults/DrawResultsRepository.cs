using System.Globalization;
using Azure.Data.Tables;
using Lotto.Storage.Entities;

namespace Lotto.Features.AddLatestDrawResults;

internal interface IDrawResultsRepository
{
    Task<DrawResultsEntity> GetLatestAsync(CancellationToken cancellationToken);

    Task AddAsync(DrawResults data, CancellationToken cancellationToken);
}

internal sealed class DrawResultsRepository(
    TableServiceClient tableServiceClient,
    IRowKeyGenerator rowKeyGenerator) : IDrawResultsRepository
{
    private const string PartitionKey = "LottoData";

    public async Task<DrawResultsEntity> GetLatestAsync(CancellationToken cancellationToken)
    {
        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);

        var entity = await client.QueryAsync<DrawResultsEntity>(maxPerPage: 1, cancellationToken: cancellationToken)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return entity ?? throw new InvalidOperationException("No DrawResults found");
    }

    public async Task AddAsync(DrawResults data, CancellationToken cancellationToken)
    {
        var drawDate = DateTime.Parse(data.DrawDate, CultureInfo.InvariantCulture);
        var rowKey = rowKeyGenerator.GenerateRowKey(drawDate);

        var entity = new DrawResultsEntity
        {
            PartitionKey = PartitionKey,
            RowKey = rowKey,
            DrawDate = data.DrawDate,
            LottoNumbers = data.LottoNumbersString,
            PlusNumbers = data.PlusNumbersString
        };

        var client = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        await client.AddEntityAsync(entity, cancellationToken);
    }
}
