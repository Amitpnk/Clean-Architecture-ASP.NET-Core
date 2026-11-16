using System.ComponentModel.DataAnnotations;

namespace CleanArch.App.Models;

// Form models carry client-side validation that mirrors the API's rules, so most mistakes are caught
// before a request is sent. The API remains the source of truth and its errors are shown as well.

public class LoginModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterModel
{
    [Required, Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6), Display(Name = "User name")]
    public string UserName { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class CategoryFormModel
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}

public class EventFormModel
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // The API stores event dates as plain (zone-less) date-times. A Local-kind value would be serialized
    // with the browser's UTC offset and shifted by the server, so the kind is always forced to Unspecified.
    private DateTime date = DateTime.Today.AddDays(7).AddHours(18);

    [Required]
    public DateTime Date
    {
        get => DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
        set => date = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
    }

    [Required, NotEmptyGuid(ErrorMessage = "Category is required.")]
    public Guid CategoryId { get; set; }
}

public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object value) => value is Guid guid && guid != Guid.Empty;
}
