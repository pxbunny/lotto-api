using System.Text.Json;
using Lotto;
using Lotto.Data;
using Lotto.Functions.Http.GetDrawResults;
using Lotto.Lotto;
using Lotto.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var jsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
    WriteIndented = false
};

builder.Services.AddSingleton(jsonSerializerOptions);

builder.Services
    .AddScoped<IDrawResultsRepository, DrawResultsRepository>()
    .AddScoped<IDrawResultsService, DrawResultsService>()
    .AddScoped<IDataSyncService, DataSyncService>();

builder.Services.AddScoped<IGetDrawResultsResponseHandler, GetDrawResultsResponseHandler>();
builder.Services.AddSingleton<IRowKeyGenerator, RowKeyGenerator>();

builder.Services.AddSingleton<IContentNegotiator<ContentType>>(new ContentNegotiator<ContentType>(config =>
    {
        config.Add("*/*", ContentType.ApplicationJson);
        config.Add("application/*", ContentType.ApplicationJson);
        config.Add("application/json", ContentType.ApplicationJson);
        config.Add("application/octet-stream", ContentType.ApplicationOctetStream);
    }));

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureWebJobsStorage"]);
});

builder.Services.AddHttpClient<ILottoClient, LottoClient>(client =>
{
    const string baseUrlPropertyName = "LottoBaseUrl";
    const string apiKeyPropertyName = "LottoApiKey";

    var baseUrlConfigValue = builder.Configuration[baseUrlPropertyName];
    var apiKeyConfigValue = builder.Configuration[apiKeyPropertyName];

    var baseUrl = !string.IsNullOrWhiteSpace(baseUrlConfigValue)
        ? baseUrlConfigValue : throw new Exception($"'{baseUrlPropertyName}' missing in the configuration.");
    var apiKey = !string.IsNullOrWhiteSpace(apiKeyConfigValue)
        ? apiKeyConfigValue : throw new Exception($"'{apiKeyPropertyName}' missing in the configuration.");

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("secret", apiKey);
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<DevDrawResultsTableSeeder>();
}

builder.Build().Run();
