using System.Globalization;
using Azure;
using Azure.Data.Tables;
using Lotto.Storage.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Lotto.Storage;

sealed class DrawResultsSeeder(
    IWebHostEnvironment env,
    TableServiceClient tableServiceClient,
    IRowKeyGenerator rowKeyGenerator) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!env.IsDevelopment()) return;

        const int seedDateRangeInYears = 10;

        var tableClient = tableServiceClient.GetTableClient(Constants.DrawResultsTableName);
        var tableAlreadyExists = !await TryCreateTableAsync(tableClient, cancellationToken);

        if (tableAlreadyExists) return;

        var seedStartDate = DateTime.Now.AddYears(-seedDateRangeInYears);
        var draws = GenerateDrawSchedule(seedStartDate);
        var now = DateTime.UtcNow;
        var random = new Random();

        foreach (var drawDate in draws)
        {
            var lotto = GenerateNumbers(random);
            var plus = GenerateNumbers(random);
            var rowKey = rowKeyGenerator.GenerateRowKey(drawDate);

            var entity = new DrawResultsEntity
            {
                PartitionKey = "LottoData",
                RowKey = rowKey,
                Timestamp = now,
                DrawDate = drawDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture),
                LottoNumbers = string.Join(",", lotto),
                PlusNumbers = string.Join(",", plus)
            };

            await tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    static async Task<bool> TryCreateTableAsync(TableClient tableClient, CancellationToken cancellationToken)
    {
        try
        {
            await tableClient.CreateAsync(cancellationToken);
            return true;
        }
        catch (RequestFailedException ex) when (ex.Status == 409)
        {
            return false;
        }
    }

    static IEnumerable<DateTime> GenerateDrawSchedule(DateTime startDate)
    {
        var date = startDate.Date;

        while (date < DateTime.Now.Date)
        {
            var dayOfWeek = date.DayOfWeek;

            if (dayOfWeek is DayOfWeek.Tuesday or DayOfWeek.Thursday or DayOfWeek.Saturday)
                yield return new DateTime(date.Year, date.Month, date.Day);

            date = date.AddDays(1);
        }
    }

    static IEnumerable<int> GenerateNumbers(Random random)
    {
        const int numbersInSingleDraw = 6;
        var set = new HashSet<int>();

        while (set.Count < numbersInSingleDraw)
            set.Add(random.Next(1, 50)); // between 1 and 49

        return set.ToList().Order();
    }
}
