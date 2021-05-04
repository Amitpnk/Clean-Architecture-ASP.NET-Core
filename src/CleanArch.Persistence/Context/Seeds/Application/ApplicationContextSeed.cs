using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CleanArch.Persistence.Context.Seeds.Application
{
    public static class ApplicationContextSeed
    {

        static Guid concertGuid = Guid.NewGuid();
        static Guid musicalGuid = Guid.NewGuid();
        static Guid conferenceGuid = Guid.NewGuid();

        public static void ApplicationSeed(this ModelBuilder modelBuilder)
        {
            CreateCategory(modelBuilder);
            CreateEvent(modelBuilder);
        }

        private static void CreateEvent(ModelBuilder modelBuilder)
        {
            List<Event> events = EventsList();
            modelBuilder.Entity<Event>().HasData(events);
        }

        private static void CreateCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(CategoryList());
        }

        private static List<Category> CategoryList()
        {
            return new List<Category>()
            {
                new Category() {Id=concertGuid  , Name= "Concert", Description= "Concert"},
                new Category() {Id=musicalGuid  , Name= "Musical", Description= "Musical"},
                new Category() {Id=conferenceGuid, Name= "Conference", Description= "Conference"}
            };
        }

        private static List<Event> EventsList()
        {
            return new List<Event>()
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Guitar hits 2020",
                    Date = DateTime.Now.AddMonths(4),
                    Description = "Guitar music concert 2020",
                    CategoryId = musicalGuid
                },

                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Guitar hits 2021",
                    Date = DateTime.Now.AddMonths(4),
                    Description = "Guitar music concert 2021",
                    CategoryId = musicalGuid
                },

                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Event 2020",
                    Date = DateTime.Now.AddMonths(10),
                    Description = "The tech conference in c#",
                    CategoryId = conferenceGuid
                },

                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Event 2021",
                    Date = DateTime.Now.AddMonths(8),
                    Description = "The tech conference in .net core",
                    CategoryId = conferenceGuid
                },
            };
        }

    }
}