using CleanArch.Application.CQRS;
using CleanArch.Application.Features.Categories.Commands.UpdateCategory;
using CleanArch.Application.Features.Events.Commands.UpdateEvent;
using FluentValidation;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application.UnitTests;

public class ApplicationServiceRegistrationTests
{
    [Fact]
    public void AddApplicationServices_RegistersDispatcherMapperAndValidators()
    {
        using var provider = new ServiceCollection().AddApplicationServices().BuildServiceProvider();
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetService<IDispatcher>());
        Assert.NotNull(scope.ServiceProvider.GetService<IMapper>());
        Assert.IsType<UpdateCategoryCommandValidator>(scope.ServiceProvider.GetService<IValidator<UpdateCategoryCommand>>());
        Assert.IsType<UpdateEventCommandValidator>(scope.ServiceProvider.GetService<IValidator<UpdateEventCommand>>());
    }
}
