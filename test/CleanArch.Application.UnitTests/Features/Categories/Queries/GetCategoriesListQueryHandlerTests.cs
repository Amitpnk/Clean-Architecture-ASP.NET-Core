using AutoMapper;
using CleanArch.Application.Contracts.Persistence;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;
using CleanArch.Application.Profiles;
using CleanArch.Common.UnitTests.Mocks;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArch.Application.UnitTests.Features.Categories.Queries
{
    public class GetCategoriesListQueryHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;

        public GetCategoriesListQueryHandlerTests()
        {
            _mockCategoryRepository = RepositoryMocks.GetCategoryRepository();
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Test]
        public async Task GetCategoriesListTest()
        {
            var handler = new GetCategoriesListQueryHandler(_mapper, _mockCategoryRepository.Object);

            var result = await handler.Handle(new GetCategoriesListQuery(), CancellationToken.None);

            Assert.AreEqual(4, result.Count);

            Assert.That(result, Is.TypeOf<List<CategoryListVm>>());
        }
    }
}
