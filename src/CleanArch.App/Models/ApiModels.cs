namespace CleanArch.App.Models;

// Client-side copies of the API's response shapes. Property names match the API's JSON (camelCase,
// bound case-insensitively), so these stay decoupled from the server's Application/Domain assemblies.

public interface IApiResponse
{
    bool Success { get; }
    string Message { get; }
    List<string> ValidationErrors { get; }
}

public class BaseResponse<T> : IApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<string> ValidationErrors { get; set; }
    public T Data { get; set; }
}

public class AuthenticationResponse
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = [];
    public bool IsVerified { get; set; }
    public string JWToken { get; set; }
}

public class CategoryListVm
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
}

public class CategoryEventListVm
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public List<CategoryEventDto> Events { get; set; } = [];
}

public class CategoryEventDto
{
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
}

public class CreateCategoryResponse : BaseResponse<string>
{
    public CategoryListVm Category { get; set; }
}

public class EventListVm
{
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
}

public class EventDetailVm
{
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
    public EventCategoryDto Category { get; set; }
}

public class EventCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public sealed record FileDownload(string FileName, string ContentType, byte[] Content);
