using System.Net;
using System.Net.Http.Json;
using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;

namespace CleanArch.Api.IntegrationTests.Endpoints;

public class CategoryEndpointTests(CleanArchApiFactory factory)
{
    private const string CategoriesUrl = "/api/v1/Category";

    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task GetAllCategories_WithoutToken_Returns401()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"{CategoriesUrl}/all", CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsSeededCategories()
    {
        using var client = await factory.CreateAuthenticatedClientAsync(CancellationToken);

        var categories = await client.GetFromJsonAsync<List<CategoryListVm>>($"{CategoriesUrl}/all", CancellationToken);

        Assert.Subset(categories!.Select(c => c.Name).ToHashSet(), new HashSet<string> { "Concert", "Musical", "Conference" });
    }

    [Fact]
    public async Task CreateCategory_PersistsCategory_AndRejectsDuplicateName()
    {
        using var client = await factory.CreateAuthenticatedClientAsync(CancellationToken);
        var name = $"Cat-{Guid.NewGuid():N}"[..20];

        var first = await client.PostAsJsonAsync(CategoriesUrl, new CreateCategoryCommand { Name = name }, CancellationToken);
        first.EnsureSuccessStatusCode();
        var created = await first.Content.ReadFromJsonAsync<CreateCategoryCommandResponse>(CancellationToken);

        var categories = await client.GetFromJsonAsync<List<CategoryListVm>>($"{CategoriesUrl}/all", CancellationToken);

        var duplicate = await client.PostAsJsonAsync(CategoriesUrl, new CreateCategoryCommand { Name = name }, CancellationToken);
        var duplicateBody = await duplicate.Content.ReadFromJsonAsync<CreateCategoryCommandResponse>(CancellationToken);

        Assert.True(created!.Success);
        Assert.NotEqual(Guid.Empty, created.Category.CategoryId);
        Assert.Contains(name, categories!.Select(c => c.Name));
        Assert.False(duplicateBody!.Success);
        Assert.NotEmpty(duplicateBody.ValidationErrors);
    }

    [Fact]
    public async Task GetCategoriesWithEvents_ReturnsOk()
    {
        using var client = await factory.CreateAuthenticatedClientAsync(CancellationToken);

        var response = await client.GetAsync($"{CategoriesUrl}/allwithevents?includeHistory=true", CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
