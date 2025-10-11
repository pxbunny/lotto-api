using Microsoft.Extensions.DependencyInjection;

namespace Lotto.Features.DropDrawResultsTable;

internal static class DependencyInjection
{
    public static void Add_DropDrawResultsTable_Feature(this IServiceCollection services)
    {
        services.AddScoped<IDrawResultsRepository, DrawResultsRepository>();
    }
}
