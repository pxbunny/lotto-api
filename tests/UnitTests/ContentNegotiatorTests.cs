using Lotto.Features.Http.GetDrawResults.FunctionHelpers;
using Microsoft.AspNetCore.Http;

namespace Lotto.UnitTests;

public sealed class ContentNegotiatorTests
{
    [Theory]
    // Exact matches
    [InlineData("application/json", true, ContentType.ApplicationJson)]
    [InlineData("APPLICATION/JSON", true, ContentType.ApplicationJson)] // Case-insensitive
    [InlineData("application/octet-stream", true, ContentType.ApplicationOctetStream)]
    [InlineData("application/octet-stream; q=0.9", true, ContentType.ApplicationOctetStream)]
    // Q-value ordering
    [InlineData("application/json; q=0.8, application/octet-stream; q=0.9", true, ContentType.ApplicationOctetStream)]
    [InlineData("application/octet-stream; q=0.8, application/json; q=0.9", true, ContentType.ApplicationJson)]
    // Wildcards
    [InlineData("*/*", true, ContentType.ApplicationJson)]
    [InlineData("application/*", true, ContentType.ApplicationJson)]
    [InlineData("text/html, application/*", true, ContentType.ApplicationJson)]
    // Multiple values
    [InlineData("text/html, application/json", true, ContentType.ApplicationJson)]
    [InlineData("application/octet-stream, */*", true, ContentType.ApplicationOctetStream)]
    // Edge cases
    [InlineData("somethingapplication/json", false)] // Not a valid media type
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("text/plain", false)]
    [InlineData("application/xml", false)]
    [InlineData("application/json; version=2", true, ContentType.ApplicationJson)] // Ignores parameters
    void Negotiate_VariousAcceptHeaders_ReturnsExpected(
        string? acceptHeader,
        bool success,
        ContentType? expected = null)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var negotiator = new ContentNegotiator<ContentType>(config =>
        {
            config.Add("*/*", ContentType.ApplicationJson);
            config.Add("application/*", ContentType.ApplicationJson);
            config.Add("application/json", ContentType.ApplicationJson);
            config.Add("application/octet-stream", ContentType.ApplicationOctetStream);
        });

        if (acceptHeader is not null)
            context.Request.Headers.Accept = acceptHeader;

        // Act
        var (result, contentType) = negotiator.Negotiate(context.Request);

        // Assert
        Assert.Equal(success, result);
        if (success) Assert.Equal(expected, contentType);
    }
}
