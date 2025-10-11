namespace Lotto.Interfaces;

internal interface IHandler<T>
{
    Task HandleAsync(CancellationToken cancellationToken);
}

internal interface IHandler<T, in TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}

internal interface IHandler<T, in TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
