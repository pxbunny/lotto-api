using Microsoft.AspNetCore.Mvc;

namespace Lotto.Functions.Http.DevStorageManagement;

internal sealed class CreateDrawResultsTableFunction(IDrawResultsRepository repository)
{
    [Function(nameof(CreateDrawResultsTableFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "draw-results-table")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        await repository.CreateTableIfNotExistsAsync(cancellationToken);
        return new OkResult();
    }
}
