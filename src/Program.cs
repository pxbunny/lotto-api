using System.Text.Json;
using Lotto;
using Lotto.LottoClient;
using Lotto.Storage;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
    WriteIndented = false
});

builder.Services.AddStorage(builder.Configuration, builder.Environment);
builder.Services.AddLottoClient(builder.Configuration);

builder.RunFeatureInstallers();

builder.Build().Run();
