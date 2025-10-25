using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lotto.LottoClient;

static class DependencyInjection
{
    public static void AddLottoClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ILottoClient, LottoClient>(client =>
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
