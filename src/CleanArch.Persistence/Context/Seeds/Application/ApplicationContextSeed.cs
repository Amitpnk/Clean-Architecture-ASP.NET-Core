using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.Context.Seeds.Application;

public static class ApplicationContextSeed
{

    // Fixed keys and dates: HasData seed values must be deterministic or EF sees a model change on every run.
    private static readonly Guid concertGuid = new("b0f5c1a2-3d4e-4f60-8a71-92b3c4d5e601");
    private static readonly Guid musicalGuid = new("b0f5c1a2-3d4e-4f60-8a71-92b3c4d5e602");
    private static readonly Guid conferenceGuid = new("b0f5c1a2-3d4e-4f60-8a71-92b3c4d5e603");

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
                    Id = new Guid("e1a2b3c4-d5e6-4f70-8192-a3b4c5d6e701"),
                    Name = "Guitar hits 2020",
                    Date = new DateTime(2027, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Guitar music concert 2020",
                    CategoryId = musicalGuid
                },

                new Event
                {
                    Id = new Guid("e1a2b3c4-d5e6-4f70-8192-a3b4c5d6e702"),
                    Name = "Guitar hits 2021",
                    Date = new DateTime(2027, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Guitar music concert 2021",
                    CategoryId = musicalGuid
                },

                new Event
                {
                    Id = new Guid("e1a2b3c4-d5e6-4f70-8192-a3b4c5d6e703"),
                    Name = "Event 2020",
                    Date = new DateTime(2027, 9, 15, 0, 0, 0, DateTimeKind.Utc),
                    Description = "The tech conference in c#",
                    CategoryId = conferenceGuid
                },

                new Event
                {
                    Id = new Guid("e1a2b3c4-d5e6-4f70-8192-a3b4c5d6e704"),
                    Name = "Event 2021",
                    Date = new DateTime(2027, 7, 15, 0, 0, 0, DateTimeKind.Utc),
                    Description = "The tech conference in .net core",
                    CategoryId = conferenceGuid
                },
            };
    }

}