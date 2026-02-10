using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace Lotto.Features.Http.GetDrawResultsByDate;

internal sealed class HttpGetFunction(
    FunctionHandler handler,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger<HttpGetFunction> logger)
{
    private const string FunctionName = "GetDrawResultsByDate";

    [Function(FunctionName)]
    [OpenApiOperation(FunctionName, "Draw Results")]
    [OpenApiParameter("date", In = ParameterLocation.Path)]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(DrawResultsDto))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ErrorResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(string))]
    [FunctionKeySecurity]
    public async Task<IActionResult> Run(
        [HttpTrigger("get", Route = "draw-results/{date:datetime}")] HttpRequest _,
        string date,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("{FunctionName} handling request for date {Date}", FunctionName, date);
            var response = await HandleRequestAsync(date, cancellationToken);
            logger.LogInformation("{FunctionName} finished successfully.", FunctionName);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, "{FunctionName} failed while processing request.", FunctionName);
            throw;
        }
    }

    private async Task<IActionResult> HandleRequestAsync(string date, CancellationToken cancellationToken)
    {
        if (!TryParseDate(date, out var parsedDate, out var errorMessage))
        {
            logger.LogError("Route date validation failed: {ErrorMessage}", errorMessage);
            return new BadRequestObjectResult(new ErrorResponse(errorMessage!));
        }

        var result = await handler.HandleAsync(parsedDate, cancellationToken);

        if (result is null)
        {
            return new NotFoundObjectResult("No draw results found for the given date.");
        }

        var dto = new DrawResultsDto(result.DrawDate, result.LottoNumbers, result.PlusNumbers);

        return new ContentResult
        {
            Content = JsonSerializer.Serialize(dto, jsonSerializerOptions),
            ContentType = "application/json",
            StatusCode = 200
        };
    }

    private static bool TryParseDate(string date, out DateOnly parsedDate, out string? errorMessage)
    {
        errorMessage = null;

        var isValid = DateOnly.TryParseExact(
            date,
            Defaults.DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out parsedDate);

        if (!isValid) errorMessage = $"'date' must be a valid date in the format {Defaults.DateFormat}.";

        return isValid;
    }
}
