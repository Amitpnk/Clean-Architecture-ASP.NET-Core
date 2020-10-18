using CA.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA.Domain.Entities
{
    public class Card : AggregateRoot<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SerialNumber { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public string Description { get; set; }
        public string Synonmys { get; set; }
        public string Meaning { get; set; }
        public Group Group { get; set; }
        public Guid? GroupId { get; set; }

    }
}
