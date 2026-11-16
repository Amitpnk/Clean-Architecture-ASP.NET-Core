namespace CleanArch.Application.CQRS;

/// <summary>A read-only request that returns <typeparamref name="TResponse"/>.</summary>
public interface IQuery<out TResponse>;
