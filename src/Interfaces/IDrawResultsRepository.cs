using Lotto.Data;

namespace Lotto.Interfaces;

internal interface IDrawResultsRepository
{
    Task AddAsync(DrawResults drawResults, CancellationToken cancellationToken);

    Task<DrawResultsEntity> GetLatestAsync(CancellationToken cancellationToken);

    Task<IEnumerable<DrawResultsEntity>> GetAsync(string filter, int top, CancellationToken cancellationToken);
}
