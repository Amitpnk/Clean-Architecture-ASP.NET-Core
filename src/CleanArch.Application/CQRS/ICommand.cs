namespace CleanArch.Application.CQRS;

/// <summary>Marker for every command, so pipeline behaviors can target commands only.</summary>
public interface IBaseCommand;

/// <summary>A command that changes state and returns nothing.</summary>
public interface ICommand : ICommand<Unit>;

/// <summary>A command that changes state and returns <typeparamref name="TResponse"/>.</summary>
public interface ICommand<out TResponse> : IBaseCommand;
