using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application.CQRS;

public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    // One closed-generic invoker per request type: reflection runs once, then the call is strongly typed.
    private static readonly ConcurrentDictionary<Type, object> Invokers = new();

    public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var invoker = (IRequestInvoker<TResponse>)Invokers.GetOrAdd(
            command.GetType(),
            static type => Activator.CreateInstance(typeof(CommandInvoker<,>).MakeGenericType(type, typeof(TResponse)))!);
        return invoker.Invoke(command, this, cancellationToken);
    }

    public Task<TResponse> Send<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(command);
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        return RunPipeline(command, ct => handler.Handle(command, ct), cancellationToken);
    }

    public Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        var invoker = (IRequestInvoker<TResponse>)Invokers.GetOrAdd(
            query.GetType(),
            static type => Activator.CreateInstance(typeof(QueryInvoker<,>).MakeGenericType(type, typeof(TResponse)))!);
        return invoker.Invoke(query, this, cancellationToken);
    }

    public Task<TResponse> Query<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(query);
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
        return RunPipeline(query, ct => handler.Handle(query, ct), cancellationToken);
    }

    private Task<TResponse> RunPipeline<TRequest, TResponse>(
        TRequest request,
        RequestHandlerDelegate<TResponse> handler,
        CancellationToken cancellationToken)
    {
        // Wrap inside-out so the first registered behavior is the outermost one.
        var pipeline = handler;
        foreach (var behavior in serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse())
        {
            var next = pipeline;
            pipeline = ct => behavior.Handle(request, next, ct);
        }

        return pipeline(cancellationToken);
    }

    private interface IRequestInvoker<TResponse>
    {
        Task<TResponse> Invoke(object request, Dispatcher dispatcher, CancellationToken cancellationToken);
    }

    private sealed class CommandInvoker<TCommand, TResponse> : IRequestInvoker<TResponse>
        where TCommand : ICommand<TResponse>
    {
        public Task<TResponse> Invoke(object request, Dispatcher dispatcher, CancellationToken cancellationToken) =>
            dispatcher.Send<TCommand, TResponse>((TCommand)request, cancellationToken);
    }

    private sealed class QueryInvoker<TQuery, TResponse> : IRequestInvoker<TResponse>
        where TQuery : IQuery<TResponse>
    {
        public Task<TResponse> Invoke(object request, Dispatcher dispatcher, CancellationToken cancellationToken) =>
            dispatcher.Query<TQuery, TResponse>((TQuery)request, cancellationToken);
    }
}
