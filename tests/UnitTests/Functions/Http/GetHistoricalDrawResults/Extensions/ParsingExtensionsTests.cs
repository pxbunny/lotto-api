using System.Globalization;
using LottoDrawHistory;
using LottoDrawHistory.Functions.Http.GetHistoricalDrawResults.Extensions;
using Microsoft.Extensions.Primitives;

namespace UnitTests.Functions.Http.GetHistoricalDrawResults.Extensions;

public class HttpRequestExtensionsParsingTests
{
    private const string ValidDate = "2023-12-31";
    private const string InvalidDate = "31/12/2023";
    private const string ImpossibleDate = "2023-02-30";
    private const string ValidInt = "10";
    private const string InvalidInt = "abc";
    private const string NegativeInt = "-5";

    [Theory]
    [InlineData(ValidDate, null, null, ValidDate, null, null)]
    [InlineData(null, ValidDate, null, null, ValidDate, null)]
    [InlineData(null, null, ValidInt, null, null, 10)]
    [InlineData(ValidDate, ValidDate, ValidInt, ValidDate, ValidDate, 10)]
    [InlineData(InvalidDate, null, null, null, null, null)]
    [InlineData(null, InvalidDate, null, null, null, null)]
    [InlineData(ImpossibleDate, null, null, null, null, null)]
    [InlineData(null, null, InvalidInt, null, null, null)]
    [InlineData(null, null, NegativeInt, null, null, -5)]
    [InlineData("", "", "", null, null, null)]  // Empty values
    [InlineData(ValidDate, InvalidDate, NegativeInt, ValidDate, null, -5)]
    public void ParseQueryString_VariousScenarios_ParsesCorrectly(
        string? dateFromInput,
        string? dateToInput,
        string? topInput,
        string? expectedDateFrom,
        string? expectedDateTo,
        int? expectedTop)
    {
        // Arrange
        var query = new QueryCollection(BuildQueryParams(dateFromInput, dateToInput, topInput));
        var request = new DefaultHttpContext().Request;
        request.Query = query;

        // Act
        var result = request.ParseQueryString();

        // Assert
        Assert.Equal(ParseDate(expectedDateFrom), result.DateFrom);
        Assert.Equal(ParseDate(expectedDateTo), result.DateTo);
        Assert.Equal(expectedTop, result.Top);
    }

    private static DateOnly? ParseDate(string? dateStr)
    {
        return DateOnly.TryParseExact(dateStr,
            Constants.DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date)
            ? date
            : null;
    }

    private static Dictionary<string, StringValues> BuildQueryParams(params string?[] values)
    {
        var keys = new[] { "dateFrom", "dateTo", "top" };
        var dict = new Dictionary<string, StringValues>();

        for (var i = 0; i < values.Length; i++)
        {
            if (!string.IsNullOrEmpty(values[i]))
                dict.Add(keys[i], values[i]!);
        }

        return dict;
    }
}
