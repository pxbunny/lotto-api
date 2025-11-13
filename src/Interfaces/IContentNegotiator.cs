namespace Lotto.Interfaces;

internal interface IContentNegotiator<T> where T : struct, Enum
{
    NegotiationResult<T> Negotiate(HttpRequest request);
}
