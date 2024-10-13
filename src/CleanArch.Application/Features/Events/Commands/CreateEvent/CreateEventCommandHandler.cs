using AutoMapper;
using CleanArch.Application.Contracts.Infrastructure;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Services.Mail;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Events.Commands.CreateEvent;

public class CreateEventCommandHandler(
    IMapper mapper,
    IEventRepository eventRepository,
    IEmailService emailService,
    ILogger<CreateEventCommandHandler> logger)
    : IRequestHandler<CreateEventCommand, Guid>
{
    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateEventCommandValidator(eventRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Count > 0)
        {
            throw new Exceptions.ValidationException(validationResult);
        }

        var @event = mapper.Map<Event>(request);

        @event = await eventRepository.AddAsync(@event);

        // Todo: Sending email notification to admin address
        var email = new MailRequest() { ToEmail = "amit.naik8103@gmail.com", Body = $"A new event was created: {request}", Subject = "A new event was created" };

        try
        {
            await emailService.SendEmailAsync(email);
        }
        catch (Exception ex)
        {
            //this shouldn't stop the API from doing else so this can be logged
            logger.LogError($"Mailing about event {@event.Id} failed due to an error with the mail service: {ex.Message}");
        }

        return @event.Id;
    }
}