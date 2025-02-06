using LottoDrawHistory.CQRS;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions.Timer;

sealed class AddLatestDrawResults(IMediator mediator, ILogger<AddLatestDrawResults> logger)
{
    private const string FunctionName = nameof(AddLatestDrawResults);

    [Function(FunctionName)]
    [FixedDelayRetry(3, "00:15:00")]
    public async Task Run(
        [TimerTrigger(
            "%DataUpdateSchedule%"
#if DEBUG
            //, RunOnStartup = true
#endif
            )] TimerInfo timer,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("{FunctionName} function triggered at: {TriggerTime}", FunctionName, DateTime.UtcNow);

        try
        {
            await mediator.Send(new AddLatestDrawResultsCommand(), cancellationToken);
            logger.LogInformation("{FunctionName} finished successfully.", FunctionName);
        }
        catch (Exception e)
        {
            logger.LogError("{FunctionName} Failed.", FunctionName);
            throw;
        }

        if (timer.ScheduleStatus is not null)
            logger.LogInformation("Next timer schedule at: {NextScheduledTrigger}", timer.ScheduleStatus.Next);
    }
}
