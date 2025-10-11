using Microsoft.Extensions.DependencyInjection;

namespace Lotto.Features.CreateDrawResultsTable;

internal static class DependencyInjection
{
    public static void Add_CreateDrawResultsTable_Feature(this IServiceCollection services)
    {
        services.AddScoped<IDrawResultsRepository, DrawResultsRepository>();
    }
}
