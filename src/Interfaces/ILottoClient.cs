namespace Lotto.Interfaces;

internal interface ILottoClient
{
    Task<DrawResults> GetLatestDrawResultsAsync(CancellationToken cancellationToken);
}
