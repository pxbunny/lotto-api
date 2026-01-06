using Lotto.Storage.Entities;

namespace Lotto.Interfaces;

internal interface IDrawResultsRepository
{
    Task<IEnumerable<DrawResultsEntity>> GetAsync(string filter, int top, CancellationToken cancellationToken);

    Task<DrawResultsEntity> GetLatestAsync(CancellationToken cancellationToken);

    Task AddAsync(DrawResults data, CancellationToken cancellationToken);
}
