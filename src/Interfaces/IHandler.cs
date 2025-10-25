namespace Lotto.Interfaces;

interface IHandler<T>
{
    Task HandleAsync(CancellationToken cancellationToken);
}

interface IHandler<T, in TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}

interface IHandler<T, in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
