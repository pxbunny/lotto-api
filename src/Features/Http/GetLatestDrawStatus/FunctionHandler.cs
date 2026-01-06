namespace Lotto.Features.Http.GetLatestDrawStatus;

internal sealed class FunctionHandler(
    IDrawResultsRepository repository,
    LottoClient lottoClient,
    ILogger<FunctionHandler> logger)
{
    public async Task<LatestDrawStatusDto> HandleAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetLatestDrawStatus.");

        string? storageDate = null;

        try
        {
            storageDate = (await repository.GetLatestAsync(cancellationToken)).DrawDate;
        }
        catch (InvalidOperationException)
        {
            logger.LogWarning("No draw results found in storage.");
        }

        var apiDate = (await lottoClient.GetLatestDrawResultsAsync(cancellationToken)).DrawDate;
        var isUpToDate = !string.IsNullOrWhiteSpace(storageDate) && storageDate == apiDate;

        logger.LogInformation(
            "Latest draw status - StorageDate: {StorageDate}, ApiDate: {ApiDate}, UpToDate: {UpToDate}",
            storageDate, apiDate, isUpToDate);

        return new LatestDrawStatusDto(storageDate, apiDate, isUpToDate);
    }
}
