namespace Lotto.Interfaces;

internal interface IDrawResultsService
{
    Task<IEnumerable<DrawResults>> GetDrawResultsAsync(
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int? top,
        CancellationToken cancellationToken);
}
