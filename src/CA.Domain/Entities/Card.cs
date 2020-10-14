using CA.Domain.Common;
using System;

namespace CA.Domain.Entities
{
    public class Card : AggregateRoot<Guid>
    {

        //public string Code { get; set; }
        //public string Name { get; set; }
        public string Description { get; set; }
        public string Synonmys { get; set; }
        public string Meaning { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }

        public Group Group { get; set; }
        public Guid GroupId { get; set; }

    }
}
