using LottoDrawHistory.CQRS;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions.Timer;

sealed class AddLatestDrawResults(IMediator mediator, ILogger<AddLatestDrawResults> logger)
{
    private const string FunctionName = nameof(AddLatestDrawResults);
    
    [Function(FunctionName)]
    public async Task Run(
        [TimerTrigger(
            "0 0 23 * * 2,4,6"
#if DEBUG
            //, RunOnStartup = true
#endif
            )] TimerInfo timer,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("{FunctionName} function triggered at: {TriggerTime}", FunctionName, DateTime.UtcNow);
        
        await mediator.Send(new AddLatestDrawResultsCommand(), cancellationToken);
        
        logger.LogInformation("{FunctionName} finished its job.", FunctionName);
        
        if (timer.ScheduleStatus is not null)
            logger.LogInformation("Next timer schedule at: {NextScheduledTrigger}", timer.ScheduleStatus.Next);
    }
}
