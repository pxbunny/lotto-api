namespace Lotto.Features.Timer.AddLatestDrawResults;

internal sealed class TimerFunction(FunctionHandler handler, ILogger<TimerFunction> logger)
{
    private const string FunctionName = "AddLatestDrawResults";

    [Function(FunctionName), FixedDelayRetry(3, "00:15:00")]
    public async Task Run(
#if RELEASE
        [TimerTrigger("%DataSyncSchedule%", UseMonitor = false, RunOnStartup = true)] TimerInfo timer,
#else
        [TimerTrigger("%DataSyncSchedule%", UseMonitor = false)] TimerInfo timer,
#endif
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("{FunctionName} function triggered at: {TriggerTime}", FunctionName, DateTime.UtcNow);
            await handler.HandleAsync(cancellationToken);
            logger.LogInformation("{FunctionName} finished successfully.", FunctionName);
        }
        catch (Exception e)
        {
            logger.LogError("{FunctionName} Failed. Error: {ErrorMessage}", FunctionName, e.Message);
            throw;
        }

        if (timer.ScheduleStatus is null) return;

        logger.LogInformation("Next timer schedule at: {NextScheduledTrigger}", timer.ScheduleStatus.Next);
    }
}
