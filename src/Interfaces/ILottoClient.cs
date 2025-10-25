namespace Lotto.Interfaces;

interface ILottoClient
{
    Task<DrawResults> GetLatestDrawResultsAsync(CancellationToken cancellationToken);
}
