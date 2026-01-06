using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lotto.Storage;

internal static class DependencyInjection
{
    public static void AddStorage(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddOptions<TableOptions>()
            .Bind(configuration.GetSection(TableOptions.SectionName))
            .Validate(
                o => !string.IsNullOrWhiteSpace(o.DrawResultsTableName),
                "'TableNames__DrawResults' is required.")
            .ValidateOnStart();

        services.AddSingleton<IRowKeyGenerator, RowKeyGenerator>();
        services.AddScoped<IDrawResultsRepository, DrawResultsRepository>();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient(configuration["AzureWebJobsStorage"]);
        });

        if (environment.IsDevelopment()) services.AddHostedService<DrawResultsSeeder>();
    }
}
