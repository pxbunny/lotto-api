using Lotto.Features.GetDrawResults.FunctionHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lotto.Features.GetDrawResults;

internal sealed class FeatureInstaller : IFeatureInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
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
