using System;

namespace CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents
{
    public class CategoryEventDto
    {
        public Guid EventId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
    }
}
