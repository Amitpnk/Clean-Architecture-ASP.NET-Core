namespace CleanArch.Application.CQRS;

/// <summary>The "void" result of a command that returns nothing.</summary>
public readonly record struct Unit
{
    public static readonly Unit Value = new();
}
