namespace CleanArch.Application.CQRS;

public interface IDispatcher
{
    /// <summary>Sends a command to its single handler; the response type is inferred from the command.</summary>
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    /// <summary>Sends a command whose concrete type is known at compile time (no reflection).</summary>
    Task<TResponse> Send<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>;

    /// <summary>Sends a query to its single handler; the response type is inferred from the query.</summary>
    Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

    /// <summary>Sends a query whose concrete type is known at compile time (no reflection).</summary>
    Task<TResponse> Query<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>;
}
