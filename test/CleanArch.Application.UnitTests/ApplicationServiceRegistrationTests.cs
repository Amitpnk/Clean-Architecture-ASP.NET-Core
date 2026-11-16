using CleanArch.Application.CQRS;
using CleanArch.Application.Features.Categories.Commands.UpdateCategory;
using CleanArch.Application.Features.Events.Commands.UpdateEvent;
using FluentValidation;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application.UnitTests;

public class ApplicationServiceRegistrationTests
{
    [Test]
    public void AddApplicationServices_RegistersDispatcherMapperAndValidators()
    {
        using var provider = new ServiceCollection().AddApplicationServices().BuildServiceProvider();
        using var scope = provider.CreateScope();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(scope.ServiceProvider.GetService<IDispatcher>(), Is.Not.Null);
            Assert.That(scope.ServiceProvider.GetService<IMapper>(), Is.Not.Null);
            Assert.That(scope.ServiceProvider.GetService<IValidator<UpdateCategoryCommand>>(), Is.InstanceOf<UpdateCategoryCommandValidator>());
            Assert.That(scope.ServiceProvider.GetService<IValidator<UpdateEventCommand>>(), Is.InstanceOf<UpdateEventCommandValidator>());
        }
    }
}
