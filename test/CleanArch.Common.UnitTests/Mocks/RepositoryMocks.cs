using CleanArch.Application.Contracts.Persistence;
using CleanArch.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;

namespace CleanArch.Common.UnitTests.Mocks
{

    public static class RepositoryMocks
    {
        public static Mock<ICategoryRepository> GetCategoryRepository()
        {
            var concertGuid = Guid.NewGuid();
            var musicalGuid = Guid.NewGuid();
            var playGuid = Guid.NewGuid();
            var conferenceGuid = Guid.NewGuid();

            var categories = new List<Category>
            {
                new Category
                {
                    Id = concertGuid,
                    Name = "Concerts"
                },
                new Category
                {
                    Id = musicalGuid,
                    Name = "Musicals"
                },
                new Category
                {
                    Id = conferenceGuid,
                    Name = "Conferences"
                },
                 new Category
                {
                    Id = playGuid,
                    Name = "Plays"
                }
            };

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository.Setup(repo => repo.ListAllAsync()).ReturnsAsync(categories);

            mockCategoryRepository.Setup(repo => repo.AddAsync(It.IsAny<Category>())).ReturnsAsync(
                (Category category) =>
                {
                    categories.Add(category);
                    return category;
                });

            return mockCategoryRepository;
        }
    }
}
