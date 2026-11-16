using System.Net;

namespace CleanArch.App.Services;

/// <summary>A failed API call, with the message and validation errors taken from the response.</summary>
public sealed class ApiRequestException(HttpStatusCode statusCode, string message, IReadOnlyList<string> errors = null)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public IReadOnlyList<string> Errors { get; } = errors ?? [];
}
