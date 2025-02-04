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
        var getDataFromStorageTask = drawResultsService.GetLatestAsync(cancellationToken);
        var getDataFromApiTask = lottoService.GetLatestDrawResultsAsync(cancellationToken);

        var storageData = await getDataFromStorageTask;
        var apiData = await getDataFromApiTask;

        if (storageData.DrawDate == apiData.DrawDate) return;

        await drawResultsService.AddAsync(apiData, cancellationToken);
    }
}
