namespace Lotto.Interfaces;

interface IContentNegotiator<T> where T : struct, Enum
{
    NegotiationResult<T> Negotiate(HttpRequest request);
}
