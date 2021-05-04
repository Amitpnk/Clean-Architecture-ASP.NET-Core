using CleanArch.Domain.Common;
using CleanArch.Domain.Services.Authentication;
using System.Threading.Tasks;

namespace CleanArch.Application.Contracts.Persistence
{
    public interface IAuthenticationService
    {
        Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
        Task<BaseResponse<string>> RegisterAsync(RegistrationRequest request, string origin);
        Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string code);
        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task<BaseResponse<string>> ResetPassword(ResetPasswordRequest model);

    }
}
