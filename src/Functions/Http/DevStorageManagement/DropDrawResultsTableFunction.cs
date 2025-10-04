using Microsoft.AspNetCore.Mvc;

namespace Lotto.Functions.Http.DevStorageManagement;

internal sealed class DropDrawResultsTableFunction(IDrawResultsRepository repository)
{
    [Function(nameof(DropDrawResultsTableFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger("delete", Route = "draw-results-table")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        await repository.DropTableAsync(cancellationToken);
        return new OkResult();
    }
}
