namespace Lotto.Services;

internal sealed class DataSyncService(
    IDrawResultsRepository drawResultsRepository,
    ILottoClient lottoClient,
    ILogger<DataSyncService> logger) : IDataSyncService
{
    public async Task AddLatestDrawResultsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling AddLatestDrawResultsCommand");

        var getDataFromStorageTask = drawResultsRepository.GetLatestAsync(cancellationToken);
        var getDataFromApiTask = lottoClient.GetLatestDrawResultsAsync(cancellationToken);

        try
        {
            var storageData = await getDataFromStorageTask;
            var apiData = await getDataFromApiTask;

            if (storageData.DrawDate == apiData.DrawDate)
            {
                logger.LogWarning("Data storage already has the latest lotto draw results.");
                return;
            }

            await drawResultsRepository.AddAsync(apiData, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("Error occurred while trying do add latest data to the storage: {ErrorMessage}", e.Message);
            throw;
        }
    }
}
