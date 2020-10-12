using CA.Domain.Common;
using System;
using System.Collections.Generic;

namespace CA.Domain.Entities
{
    public class Group : AggregateRoot<Guid>
    {
        public Group()
        {
            Cards = new List<Card>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid CardId { get; set; }
        public List<Card> Cards { get; set; }
    }
}
