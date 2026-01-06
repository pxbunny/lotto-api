using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lotto.Features.Timer.AddLatestDrawResults;

internal sealed class FeatureInstaller : IFeatureInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddScoped<FunctionHandler>();

        services.AddHttpClient<LottoClient>(client =>
        {
            const string baseUrlPropertyName = "LottoBaseUrl";
            const string apiKeyPropertyName = "LottoApiKey";

            var baseUrlConfigValue = configuration[baseUrlPropertyName];
            var apiKeyConfigValue = configuration[apiKeyPropertyName];

            var baseUrl = !string.IsNullOrWhiteSpace(baseUrlConfigValue)
                ? baseUrlConfigValue
                : throw new InvalidOperationException($"'{baseUrlPropertyName}' missing in the configuration.");
            var apiKey = !string.IsNullOrWhiteSpace(apiKeyConfigValue)
                ? apiKeyConfigValue
                : throw new InvalidOperationException($"'{apiKeyPropertyName}' missing in the configuration.");

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("secret", apiKey);
        });
    }
}
