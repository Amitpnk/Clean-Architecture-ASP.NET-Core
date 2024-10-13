using CleanArch.Domain.Services.Mail;

namespace CleanArch.Application.Contracts.Infrastructure;

public interface IEmailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}