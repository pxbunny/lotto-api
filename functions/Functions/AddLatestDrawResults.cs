using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LottoDrawHistory.Functions;

public class AddLatestDrawResults(ILogger<AddLatestDrawResults> logger)
{
    [Function(nameof(AddLatestDrawResults))]
    public void Run([TimerTrigger("0 0 23 * * 2,4,6")] TimerInfo myTimer)
    {
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        if (myTimer.ScheduleStatus is not null)
        {
            logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }
}
