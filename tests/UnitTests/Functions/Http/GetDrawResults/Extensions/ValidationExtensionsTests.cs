using Lotto.Functions.Http.GetDrawResults.Extensions;
using Microsoft.Extensions.Primitives;

namespace Lotto.UnitTests.Functions.Http.GetDrawResults.Extensions;

public class ValidationExtensionsTests
{
    private const string ValidDate = "2023-12-31";
    private const string InvalidDate = "31/12/2023";
    private const string ValidInt = "10";
    private const string InvalidInt = "-5";

    [Theory]
    // Valid cases
    [InlineData(null, null, null, true, null)]
    [InlineData(ValidDate, null, null, true, null)]
    [InlineData(null, ValidDate, null, true, null)]
    [InlineData(null, null, ValidInt, true, null)]
    [InlineData(ValidDate, ValidDate, ValidInt, true, null)]
    // Invalid dateFrom
    [InlineData(InvalidDate, null, null, false, "dateFrom")]
    [InlineData("", null, null, false, "dateFrom")]  // Empty value
    // Invalid dateTo
    [InlineData(null, InvalidDate, null, false, "dateTo")]
    [InlineData(null, "", null, false, "dateTo")]  // Empty value
    // Invalid top
    [InlineData(null, null, "0", false, "top")]
    [InlineData(null, null, "-5", false, "top")]
    [InlineData(null, null, "abc", false, "top")]
    [InlineData(null, null, "", false, "top")]  // Empty value
    // Priority order checks
    [InlineData(InvalidDate, InvalidDate, InvalidInt, false, "dateFrom")]
    [InlineData(ValidDate, InvalidDate, InvalidInt, false, "dateTo")]
    [InlineData(ValidDate, ValidDate, InvalidInt, false, "top")]
    public void ValidateQueryString_VariousScenarios_ReturnsExpected(
        string? dateFrom, string? dateTo, string? top, bool expectedIsValid, string? expectedErrorKey)
    {
        // Arrange
        var query = new QueryCollection(BuildQueryParams(dateFrom, dateTo, top));
        var request = new DefaultHttpContext().Request;
        request.Query = query;

        // Act
        var result = request.ValidateQueryString();

        // Assert
        Assert.Equal(expectedIsValid, result.IsValid);
        Assert.Equal(expectedErrorKey is not null, !string.IsNullOrEmpty(result.ErrorMessage));

        if (expectedErrorKey is not null)
            Assert.Contains(expectedErrorKey, result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, StringValues> BuildQueryParams(params string?[] values)
    {
        var keys = new[] { "dateFrom", "dateTo", "top" };
        var dict = new Dictionary<string, StringValues>();

        for (var i = 0; i < values.Length; i++)
        {
            if (values[i] is not null)
                dict.Add(keys[i], values[i]!);
        }

        return dict;
    }
}
