using LottoDrawHistory.Functions.Http.GetDrawResults;
using LottoDrawHistory.Functions.Http.GetDrawResults.Extensions;

namespace UnitTests.Functions.Http.GetDrawResults.Extensions;

public class HeadersExtensionsTests
{
    [Theory]
    // Exact matches
    [InlineData("application/json", ContentType.ApplicationJson)]
    [InlineData("APPLICATION/JSON", ContentType.ApplicationJson)] // Case-insensitive
    [InlineData("application/octet-stream", ContentType.ApplicationOctetStream)]
    [InlineData("application/octet-stream; q=0.9", ContentType.ApplicationOctetStream)]
    // Q-value ordering
    [InlineData("application/json; q=0.8, application/octet-stream; q=0.9", ContentType.ApplicationOctetStream)]
    [InlineData("application/octet-stream; q=0.8, application/json; q=0.9", ContentType.ApplicationJson)]
    // Wildcards
    [InlineData("*/*", ContentType.ApplicationJson)]
    [InlineData("application/*", ContentType.ApplicationJson)]
    [InlineData("text/html, application/*", ContentType.ApplicationJson)]
    // Multiple values
    [InlineData("text/html, application/json", ContentType.ApplicationJson)]
    [InlineData("application/octet-stream, */*", ContentType.ApplicationOctetStream)]
    // Edge cases
    [InlineData("somethingapplication/json", null)] // Not a valid media type
    [InlineData("", null)]
    [InlineData(null, null)]
    [InlineData("text/plain", null)]
    [InlineData("application/xml", null)]
    [InlineData("application/json; version=2", ContentType.ApplicationJson)] // Ignores parameters
    private void GetAcceptHeader_VariousAcceptHeaders_ReturnsExpected(string? acceptHeader, ContentType? expected)
    {
        // Arrange
        var context = new DefaultHttpContext();

        if (acceptHeader is not null)
            context.Request.Headers.Accept = acceptHeader;

        // Act
        var result = context.Request.GetAcceptHeader();

        // Assert
        Assert.Equal(expected, result);
    }
}
