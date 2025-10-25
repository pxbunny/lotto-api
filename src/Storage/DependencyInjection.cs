using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lotto.Storage;

static class DependencyInjection
{
    public static void AddStorage(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddSingleton<IRowKeyGenerator, RowKeyGenerator>();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient(configuration["AzureWebJobsStorage"]);
        });

        if (environment.IsDevelopment())
        {
            services.AddHostedService<DrawResultsSeeder>();
        }
    }
}
