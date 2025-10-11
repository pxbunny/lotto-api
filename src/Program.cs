using System.Text.Json;
using Lotto.Features.AddLatestDrawResults;
using Lotto.Features.CreateDrawResultsTable;
using Lotto.Features.DropDrawResultsTable;
using Lotto.Features.GetDrawResults;
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

builder.Services.Add_AddLatestDrawResults_Feature();
builder.Services.Add_CreateDrawResultsTable_Feature();
builder.Services.Add_DropDrawResultsTable_Feature();
builder.Services.Add_GetDrawResults_Feature();

builder.Build().Run();
