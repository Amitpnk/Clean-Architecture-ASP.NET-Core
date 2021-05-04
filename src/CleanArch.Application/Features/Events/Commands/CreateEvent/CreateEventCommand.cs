using MediatR;
using System;

namespace CleanArch.Application.Features.Events.Commands.CreateEvent
{
    public class CreateEventCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public override string ToString()
        {
            return $"Event name: {Name}; On: {Date.ToShortDateString()}; Description: {Description}";
        }
    }
}
