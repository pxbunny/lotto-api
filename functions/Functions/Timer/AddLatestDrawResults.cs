using LottoDrawHistory.CQRS;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions.Timer;

sealed class AddLatestDrawResults(IMediator mediator, ILogger<AddLatestDrawResults> logger)
{
    [Function(nameof(AddLatestDrawResults))]
    public async Task Run(
        [TimerTrigger(
            "0 0 23 * * 2,4,6"
#if DEBUG
            //, RunOnStartup = true
#endif
            )] TimerInfo timer,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AddLatestDrawResultsCommand(), cancellationToken);
        
        if (timer.ScheduleStatus is not null)
            logger.LogInformation("Next timer schedule at: {NextScheduledTrigger}", timer.ScheduleStatus.Next);
    }
}
