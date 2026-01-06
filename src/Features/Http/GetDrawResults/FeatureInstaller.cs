using Lotto.Features.Http.GetDrawResults.FunctionHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lotto.Features.Http.GetDrawResults;

internal sealed class FeatureInstaller : IFeatureInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddScoped<FunctionHandler>();
        services.AddScoped<FunctionResponseHandler>();

        services.AddSingleton<IContentNegotiator<ContentType>>(new ContentNegotiator<ContentType>(config =>
        {
            config.Add("*/*", ContentType.ApplicationJson);
            config.Add("application/*", ContentType.ApplicationJson);
            config.Add("application/json", ContentType.ApplicationJson);
            config.Add("application/octet-stream", ContentType.ApplicationOctetStream);
        }));
    }
}
