using System.Net;
using System.Net.Http.Json;
using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;

namespace CleanArch.Api.IntegrationTests.Endpoints;

public class CategoryEndpointTests
{
    private const string CategoriesUrl = "/api/v1/Category";

    [Test]
    public async Task GetAllCategories_WithoutToken_Returns401()
    {
        using var client = ApiTestFixture.CreateAnonymousClient();

        var response = await client.GetAsync($"{CategoriesUrl}/all");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetAllCategories_ReturnsSeededCategories()
    {
        using var client = await ApiTestFixture.CreateAuthenticatedClientAsync();

        var categories = await client.GetFromJsonAsync<List<CategoryListVm>>($"{CategoriesUrl}/all");

        Assert.That(categories!.Select(c => c.Name), Is.SupersetOf(new[] { "Concert", "Musical", "Conference" }));
    }

    [Test]
    public async Task CreateCategory_PersistsCategory_AndRejectsDuplicateName()
    {
        using var client = await ApiTestFixture.CreateAuthenticatedClientAsync();
        var name = $"Cat-{Guid.NewGuid():N}"[..20];

        var first = await client.PostAsJsonAsync(CategoriesUrl, new CreateCategoryCommand { Name = name });
        first.EnsureSuccessStatusCode();
        var created = await first.Content.ReadFromJsonAsync<CreateCategoryCommandResponse>();

        var categories = await client.GetFromJsonAsync<List<CategoryListVm>>($"{CategoriesUrl}/all");

        var duplicate = await client.PostAsJsonAsync(CategoriesUrl, new CreateCategoryCommand { Name = name });
        var duplicateBody = await duplicate.Content.ReadFromJsonAsync<CreateCategoryCommandResponse>();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(created!.Success, Is.True);
            Assert.That(created.Category.CategoryId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(categories!.Select(c => c.Name), Does.Contain(name));
            Assert.That(duplicateBody!.Success, Is.False);
            Assert.That(duplicateBody.ValidationErrors, Is.Not.Empty);
        }
    }

    [Test]
    public async Task GetCategoriesWithEvents_ReturnsCategoriesWithTheirEvents()
    {
        using var client = await ApiTestFixture.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"{CategoriesUrl}/allwithevents?includeHistory=true");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
