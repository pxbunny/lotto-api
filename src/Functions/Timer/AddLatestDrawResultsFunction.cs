using LottoDrawHistory.Application;

namespace LottoDrawHistory.Functions.Timer;

internal sealed class AddLatestDrawResultsFunction(IMediator mediator, ILogger<AddLatestDrawResultsFunction> logger)
{
    private const string FunctionName = nameof(AddLatestDrawResultsFunction);

    [Function(FunctionName), FixedDelayRetry(3, "00:15:00")]
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
            logger.LogError("{FunctionName} Failed. Error: {ErrorMessage}", FunctionName, e.Message);
            throw;
        }

        if (timer.ScheduleStatus is not null)
            logger.LogInformation("Next timer schedule at: {NextScheduledTrigger}", timer.ScheduleStatus.Next);
    }
}
