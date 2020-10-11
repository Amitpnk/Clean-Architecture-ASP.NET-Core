using CA.Domain.Common;
using System;
using System.Collections.Generic;

namespace CA.Domain.Entities
{
    public class Card : AggregateRoot<Guid>
    {
        public Card()
        {
            Groups = new List<Group>();
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Synonmys { get; set; }
        public string Meaning { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }

        public List<Group> Groups { get; set; }
        //Example
        //code: "BG 1.1",
        //name: "Chapter 1, Verse 1",
        //description: "dhrtarastra uvaca dharma-ksetre kuru-ksetre samaveta yuyutsavah    mamakah pandavas caiva    kim akurvata sanjaya",
        //synonmys: "sanjayah--Sanjaya; uvaca--said; drstva--after seeing; tu--but; pandavaanikam--the soldiers of the Pandavas; vyudham--arranged in military phalanx; duryodhanah--King Duryodhana; tada--at that time; acaryam--the teacher; upasangamya--approaching nearby; raja--the king; vacanam--words; abravit--spoke.",
        //meaning: "Dhrtarastra said: O Sanjaya, after assembling in the place of pilgrimage at Kuruksetra, what did my sons and the sons of Pandu do, being desirous to fight?",
        //chapter: "1",
        //verse: "1",
    }
}
