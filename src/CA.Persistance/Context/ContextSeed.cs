using CA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CA.Persistance.Context
{
    public static class ContextSeed
    {

        public static void Seed(this ModelBuilder modelBuilder)
        {

            CreateCards(modelBuilder);

            CreateGroups(modelBuilder);

        }

        private static void CreateCards(ModelBuilder modelBuilder)
        {
            List<Card> cards = CardsList();
            modelBuilder.Entity<Card>().HasData(cards);
        }

        private static void CreateGroups(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().HasData(GroupList());
        }

        private static List<Card> CardsList()
        {
            return new List<Card>()
            {
                new Card() {
                    Id=Guid.NewGuid(),
                    //Code ="BG 1.1",
                    //Name = "Chapter 1, Verse 1",
                    Description="dhrtarastra uvaca dharma-ksetre kuru-ksetre samaveta yuyutsavah    mamakah pandavas caiva    kim akurvata sanjaya" ,
                    Synonmys = "sanjayah--Sanjaya; uvaca--said; drstva--after seeing; tu--but; pandavaanikam--the soldiers of the Pandavas; vyudham--arranged in military phalanx; duryodhanah--King Duryodhana; tada--at that time; acaryam--the teacher; upasangamya--approaching nearby; raja--the king; vacanam--words; abravit--spoke.",
                    Meaning  = "Dhrtarastra said: O Sanjaya, after assembling in the place of pilgrimage at Kuruksetra, what did my sons and the sons of Pandu do, being desirous to fight?",
                    Chapter= 1,
                    Verse =1
                },
                new Card() {
                    Id=Guid.NewGuid(),
                    //Code ="BG 1.2",
                    //Name = "Chapter 1, Verse 2",
                    Description= "sanjaya uvaca    drstva tu pandavanikam    vyudham duryodhanas tada    acaryam upasangamya    raja vacanam abravit",
                    Synonmys= "pasya--behold; etam--this; pandu-putranam--of the sons of Pandu; acarya--O teacher; mahatim--great; camum--military force; vyudham--arranged; drupada-putrena--by the son of Drupada; tava--your; sisyena--disciple; dhi-mata--very intelligent.",
                    Meaning= "Sanjaya said: O King, after looking over the army gathered by the sons of Pandu, King Duryodhana went to his teacher and began to speak the following words:",
                    Chapter= 1,
                    Verse =2
                },
                new Card() {
                    Id=Guid.NewGuid(),
                    //Code ="BG 1.3",
                    //Name = "Chapter 1, Verse 3" ,
                    Description= "pasyaitam pandu-putranam     acarya mahatim camum    vyudham drupada-putrena    tava sisyena dhimata",
                    Synonmys= "pasya--behold; etam--this; pandu-putranam--of the sons of Pandu;   acarya--O teacher; mahatim--great; camum--military force; vyudham--    arranged; drupada-putrena--by the son of Drupada; tava--your; sisyena--    disciple; dhi-mata--very intelligent.",
                    Meaning= "O my teacher, behold the great army of the sons of Pandu, so expertly    arranged by your intelligent disciple, the son of Drupada.",
                    Chapter= 1,
                    Verse =3
                },
            };
        }

        private static List<Group> GroupList()
        {
            return new List<Group>()
            {
                new Group() {Id=Guid.NewGuid(), Name= "Anger", Description= "Anger", IsActive= true},
                new Group() {Id=Guid.NewGuid(), Name= "Confusion", Description= "Confusion", IsActive= true},
                new Group() {Id=Guid.NewGuid(), Name= "Envy", Description= "Envy", IsActive= true}
            };
        }



    }
}
