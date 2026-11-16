namespace CleanArch.Domain.Common;

public class BaseResponse<T>(string message, bool success)
{
    public BaseResponse() : this(null, true)
    {
    }
    public BaseResponse(string message) : this(message, true)
    {
    }

    public BaseResponse(T data, string message = null) : this(message, true)
    {
        Data = data;
    }

    public bool Success { get; set; } = success;
    public string Message { get; set; } = message;
    public List<string> ValidationErrors { get; set; }
    public T Data { get; set; }
}