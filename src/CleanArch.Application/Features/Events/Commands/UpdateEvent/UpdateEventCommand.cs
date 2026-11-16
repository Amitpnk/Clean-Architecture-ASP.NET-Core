using CleanArch.Application.CQRS;

namespace CleanArch.Application.Features.Events.Commands.UpdateEvent;

public class UpdateEventCommand : ICommand
{
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
}