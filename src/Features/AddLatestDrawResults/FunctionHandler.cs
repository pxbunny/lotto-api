namespace Lotto.Features.AddLatestDrawResults;

sealed class FunctionHandler(
    IDrawResultsRepository repository,
    ILottoClient lottoClient,
    ILogger<FunctionHandler> logger) : IHandler<FunctionHandler>
{
    const string FunctionName = "AddLatestDrawResults";

    public async Task HandleAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Running {FunctionName} function handler.");

        var getDataFromStorageTask = repository.GetLatestAsync(cancellationToken);
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

            await repository.AddAsync(apiData, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("Error occurred while trying do add latest data to the storage: {ErrorMessage}", e.Message);
            throw;
        }

        logger.LogInformation($"{FunctionName} function handler finished.");
    }
}
