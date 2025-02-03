using LottoDrawHistory.Data;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
var assembly = typeof(IAssemblyFlag).Assembly;

builder.ConfigureFunctionsWebApplication();

builder.Services.AddScoped<DrawResultsService>();

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
