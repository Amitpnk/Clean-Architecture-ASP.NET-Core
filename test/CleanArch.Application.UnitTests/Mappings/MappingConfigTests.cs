using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;
using CleanArch.Application.Features.Events.Commands.CreateEvent;
using CleanArch.Application.Features.Events.Queries.GetEventDetail;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CleanArch.Application.Features.Events.Queries.GetEventsList;
using CleanArch.Application.UnitTests.Common;
using CleanArch.Domain.Entities;
using MapsterMapper;

namespace CleanArch.Application.UnitTests.Mappings;

public class MappingConfigTests
{
    private IMapper _mapper;
    private Event _event;
    private Category _category;

    [SetUp]
    public void SetUp()
    {
        _mapper = MapperFactory.Create();
        _category = new Category { Id = Guid.NewGuid(), Name = "Concert", Description = "Live music" };
        _event = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Rock night",
            Description = "Loud",
            Date = new DateTime(2027, 6, 1),
            CategoryId = _category.Id
        };
    }

    [Test]
    public void Event_To_EventListVm_MapsIdToEventId()
    {
        var vm = _mapper.Map<EventListVm>(_event);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.EventId, Is.EqualTo(_event.Id));
            Assert.That(vm.Name, Is.EqualTo(_event.Name));
            Assert.That(vm.Date, Is.EqualTo(_event.Date));
        }
    }

    [Test]
    public void Event_To_EventDetailVm_MapsIdToEventId()
    {
        var vm = _mapper.Map<EventDetailVm>(_event);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vm.EventId, Is.EqualTo(_event.Id));
            Assert.That(vm.Description, Is.EqualTo(_event.Description));
            Assert.That(vm.CategoryId, Is.EqualTo(_event.CategoryId));
        }
    }

    [Test]
    public void Event_To_EventExportDto_MapsIdToEventId()
    {
        var dto = _mapper.Map<EventExportDto>(_event);

        Assert.That(dto.EventId, Is.EqualTo(_event.Id));
    }

    [Test]
    public void Category_To_CategoryListVm_And_CreateCategoryDto_MapIdToCategoryId()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_mapper.Map<CategoryListVm>(_category).CategoryId, Is.EqualTo(_category.Id));
            Assert.That(_mapper.Map<CreateCategoryDto>(_category).CategoryId, Is.EqualTo(_category.Id));
        }
    }

    [Test]
    public void Category_To_CategoryDto_MapsByConvention()
    {
        var dto = _mapper.Map<CategoryDto>(_category);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(dto.Id, Is.EqualTo(_category.Id));
            Assert.That(dto.Name, Is.EqualTo(_category.Name));
        }
    }

    [Test]
    public void CreateEventCommand_To_Event_MapsByConvention()
    {
        var command = new CreateEventCommand { Name = "Expo", Description = "d", Date = new DateTime(2027, 1, 1), CategoryId = Guid.NewGuid() };

        var entity = _mapper.Map<Event>(command);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.Name, Is.EqualTo(command.Name));
            Assert.That(entity.Description, Is.EqualTo(command.Description));
            Assert.That(entity.Date, Is.EqualTo(command.Date));
            Assert.That(entity.CategoryId, Is.EqualTo(command.CategoryId));
        }
    }

    [Test]
    public void CreateEventCommand_ToString_DescribesTheEvent()
    {
        var command = new CreateEventCommand { Name = "Expo", Description = "d", Date = new DateTime(2027, 1, 1) };

        Assert.That(command.ToString(), Does.StartWith("Event name: Expo; On: ").And.EndWith("; Description: d"));
    }
}
