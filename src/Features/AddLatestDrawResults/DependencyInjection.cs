using Microsoft.Extensions.DependencyInjection;

namespace Lotto.Features.AddLatestDrawResults;

internal static class DependencyInjection
{
    public static void Add_AddLatestDrawResults_Feature(this IServiceCollection services)
    {
        services.AddScoped<IDrawResultsRepository, DrawResultsRepository>();
        services.AddScoped<IHandler<FunctionHandler>, FunctionHandler>();
    }
}
