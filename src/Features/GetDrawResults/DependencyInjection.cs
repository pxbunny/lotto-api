using Lotto.Features.GetDrawResults.FunctionHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace Lotto.Features.GetDrawResults;

internal static class DependencyInjection
{
    public static void Add_GetDrawResults_Feature(this IServiceCollection services)
    {
        services.AddScoped<IDrawResultsRepository, DrawResultsRepository>();
        services.AddScoped<IHandler<FunctionHandler, Request, IEnumerable<DrawResults>>, FunctionHandler>();
        services.AddScoped<IFunctionResponseHandler, FunctionResponseHandler>();

        services.AddSingleton<IContentNegotiator<ContentType>>(new ContentNegotiator<ContentType>(config =>
        {
            config.Add("*/*", ContentType.ApplicationJson);
            config.Add("application/*", ContentType.ApplicationJson);
            config.Add("application/json", ContentType.ApplicationJson);
            config.Add("application/octet-stream", ContentType.ApplicationOctetStream);
        }));
    }
}
