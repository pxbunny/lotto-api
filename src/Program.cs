using System.Text.Json;
using LottoDrawHistory;
using LottoDrawHistory.Data;
using LottoDrawHistory.Lotto;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
var assembly = typeof(IAssemblyFlag).Assembly;

builder.ConfigureFunctionsWebApplication();

var jsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
    WriteIndented = false
};

builder.Services.AddSingleton(jsonSerializerOptions);

builder.Services.AddScoped<DrawResultsService>();
builder.Services.AddScoped<IContentNegotiator, ContentNegotiator>();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureWebJobsStorage"]);
});

builder.Services.AddHttpClient<LottoService>(client =>
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

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(assembly);
});

builder.Build().Run();

internal interface IAssemblyFlag;
