using CA.Domain.Common;
using System;

namespace CA.Domain.Entities
{
    public class Group : AggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Card Card { get; set; }
        public Guid CardId { get; set; }
    }
}
