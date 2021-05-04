using System.ComponentModel.DataAnnotations;

namespace CleanArch.Domain.Services.Authentication
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
