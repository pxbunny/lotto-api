using System.Globalization;
using Lotto.Storage.Entities;

namespace Lotto.Features.Http.GetDrawResultsByDate;

internal sealed class FunctionHandler(IDrawResultsRepository repository, ILogger<FunctionHandler> logger)
{
    public async Task<DrawResults?> HandleAsync(DateOnly date, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetDrawResultsByDate - Date: {Date}", date);

        var filter = $"DrawDate eq '{date.ToString(Defaults.DateFormat, CultureInfo.InvariantCulture)}'";

        logger.LogInformation("Final query filter: {Filter}", filter);
        logger.LogInformation("Fetching results from storage...");

        var result = (await repository.GetAsync(filter, 1, cancellationToken)).FirstOrDefault();

        if (result is null)
        {
            logger.LogWarning("No draw results found for the given date.");
            return null;
        }

        return result.ToDrawResults();
    }
}
