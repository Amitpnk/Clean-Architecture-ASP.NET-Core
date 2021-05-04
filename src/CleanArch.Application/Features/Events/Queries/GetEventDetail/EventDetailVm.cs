using System;

namespace CleanArch.Application.Features.Events.Queries.GetEventDetail
{
    public class EventDetailVm
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryDto Category { get; set; }
    }
}
