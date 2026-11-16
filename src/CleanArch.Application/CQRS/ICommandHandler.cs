namespace CleanArch.Application.CQRS;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>Base class for a command without a result: implement <see cref="HandleAsync"/>.</summary>
public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand
{
    public async Task<Unit> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        await HandleAsync(command, cancellationToken);
        return Unit.Value;
    }

    protected abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}
