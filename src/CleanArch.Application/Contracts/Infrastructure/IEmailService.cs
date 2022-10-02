using CleanArch.Domain.Services.Mail;
using System.Threading.Tasks;

namespace CleanArch.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
