using System.Text.Json;
using LottoDrawHistory.Data;
using LottoDrawHistory.Functions.Http.Shared;
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

builder.Services
    .AddSingleton(jsonSerializerOptions);

builder.Services
    .AddScoped<DrawResultsService>()
    .AddScoped(typeof(HttpRequestHandler<>));

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureWebJobsStorage"]);
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(assembly);
});

builder.Build().Run();

interface IAssemblyFlag;
