using CleanArch.Application.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application.UnitTests.CQRS;

public class DispatcherTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public sealed record Ping(string Message) : IQuery<string>;

    public sealed class PingHandler : IQueryHandler<Ping, string>
    {
        public Task<string> Handle(Ping query, CancellationToken cancellationToken = default) =>
            Task.FromResult($"pong: {query.Message}");
    }

    public sealed record Add(int Left, int Right) : ICommand<int>;

    public sealed class AddHandler : ICommandHandler<Add, int>
    {
        public Task<int> Handle(Add command, CancellationToken cancellationToken = default) =>
            Task.FromResult(command.Left + command.Right);
    }

    public sealed record Touch : ICommand;

    public sealed class TouchHandler(List<string> log) : CommandHandler<Touch>
    {
        protected override Task HandleAsync(Touch command, CancellationToken cancellationToken)
        {
            log.Add("handler");
            return Task.CompletedTask;
        }
    }

    public sealed class RecordingBehavior<TRequest, TResponse>(List<string> log, string name)
        : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            log.Add($"{name}:before");
            var response = await next(cancellationToken);
            log.Add($"{name}:after");
            return response;
        }
    }

    private static ServiceProvider BuildProvider(List<string> log, bool withBehaviors = false)
    {
        var services = new ServiceCollection();
        services.AddSingleton(log);
        services.AddCqrs(typeof(DispatcherTests).Assembly);

        if (withBehaviors)
        {
            services.AddScoped<IPipelineBehavior<Touch, Unit>>(_ => new RecordingBehavior<Touch, Unit>(log, "outer"));
            services.AddScoped<IPipelineBehavior<Touch, Unit>>(_ => new RecordingBehavior<Touch, Unit>(log, "inner"));
        }

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    }

    [Fact]
    public async Task Query_ResolvesHandler_AndInfersResponseType()
    {
        using var provider = BuildProvider([]);
        using var scope = provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        string result = await dispatcher.Query(new Ping("hi"), Ct);

        Assert.Equal("pong: hi", result);
    }

    [Fact]
    public async Task Send_CommandWithResult_ReturnsHandlerResult()
    {
        using var provider = BuildProvider([]);
        using var scope = provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        // Both overloads: inferred (cached reflection) and explicit generic arguments.
        var inferred = await dispatcher.Send(new Add(2, 3), Ct);
        var explicitTypes = await dispatcher.Send<Add, int>(new Add(4, 5), Ct);

        Assert.Equal(5, inferred);
        Assert.Equal(9, explicitTypes);
    }

    [Fact]
    public async Task Send_RunsPipelineBehaviors_InRegistrationOrder()
    {
        var log = new List<string>();
        using var provider = BuildProvider(log, withBehaviors: true);
        using var scope = provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        var result = await dispatcher.Send(new Touch(), Ct);

        Assert.Equal(Unit.Value, result);
        Assert.Equal(["outer:before", "inner:before", "handler", "inner:after", "outer:after"], log);
    }

    [Fact]
    public async Task Send_WithoutRegisteredHandler_Throws()
    {
        using var provider = new ServiceCollection().AddCqrs().BuildServiceProvider();
        using var scope = provider.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => dispatcher.Send(new Add(1, 1), Ct));
    }

    [Fact]
    public void AddCqrs_RegistersEveryApplicationHandler()
    {
        var services = new ServiceCollection().AddCqrs(typeof(IDispatcher).Assembly);

        var handlerServices = services
            .Select(d => d.ServiceType)
            .Where(t => t.IsGenericType &&
                        (t.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                         t.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            .ToList();

        // 6 commands (create/update/delete x category/event) + 5 queries.
        Assert.Equal(11, handlerServices.Count);
    }
}
