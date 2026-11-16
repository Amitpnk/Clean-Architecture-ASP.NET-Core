namespace CleanArch.Application.CQRS;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);

/// <summary>
/// Cross-cutting step that wraps a handler (logging, validation, transactions...).
/// Register as an open generic; behaviors run for commands and queries, in registration order.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
