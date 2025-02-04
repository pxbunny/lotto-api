using LottoDrawHistory.Data;
using LottoDrawHistory.Lotto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.CQRS;

sealed record AddLatestDrawResultsCommand : IRequest;

sealed class AddLatestDrawResultsCommandHandler(
    DrawResultsService drawResultsService,
    LottoService lottoService,
    ILogger<AddLatestDrawResultsCommandHandler> logger)
    : IRequestHandler<AddLatestDrawResultsCommand>
{
    public async Task Handle(AddLatestDrawResultsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling AddLatestDrawResultsCommand");
        
        var getDataFromStorageTask = drawResultsService.GetLatestAsync(cancellationToken);
        var getDataFromApiTask = lottoService.GetLatestDrawResultsAsync(cancellationToken);

        try
        {
            var storageData = await getDataFromStorageTask;
            var apiData = await getDataFromApiTask;

            if (storageData.DrawDate == apiData.DrawDate)
            {
                logger.LogInformation("Data storage already has the latest lotto draw data. Skipping.");
                return;
            }

            await drawResultsService.AddAsync(apiData, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError("Error occurred while trying do add latest data to the storage: {ErrorMessage}", e.Message);
        }
    }
}
