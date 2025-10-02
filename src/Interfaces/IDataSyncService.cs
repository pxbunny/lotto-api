namespace Lotto.Interfaces;

internal interface IDataSyncService
{
    Task AddLatestDrawResultsAsync(CancellationToken cancellationToken);
}
