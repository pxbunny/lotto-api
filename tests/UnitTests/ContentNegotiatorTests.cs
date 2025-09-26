using LottoDrawHistory;

namespace UnitTests;

public class ContentNegotiatorTests
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
    [InlineData("somethingapplication/json", ContentType.Unsupported)] // Not a valid media type
    [InlineData("", ContentType.Unsupported)]
    [InlineData(null, ContentType.Unsupported)]
    [InlineData("text/plain", ContentType.Unsupported)]
    [InlineData("application/xml", ContentType.Unsupported)]
    [InlineData("application/json; version=2", ContentType.ApplicationJson)] // Ignores parameters
    private void Negotiate_VariousAcceptHeaders_ReturnsExpected(string? acceptHeader, ContentType expected)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var negotiator = new ContentNegotiator();

        if (acceptHeader is not null)
            context.Request.Headers.Accept = acceptHeader;

        // Act
        var result = negotiator.Negotiate(context.Request);

        // Assert
        Assert.Equal(expected, result);
    }
}
