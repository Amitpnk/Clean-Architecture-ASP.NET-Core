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
    private readonly IMapper _mapper = MapperFactory.Create();
    private readonly Category _category;
    private readonly Event _event;

    public MappingConfigTests()
    {
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

    [Fact]
    public void Event_To_EventListVm_MapsIdToEventId()
    {
        var vm = _mapper.Map<EventListVm>(_event);

        Assert.Equal(_event.Id, vm.EventId);
        Assert.Equal(_event.Name, vm.Name);
        Assert.Equal(_event.Date, vm.Date);
    }

    [Fact]
    public void Event_To_EventDetailVm_MapsIdToEventId()
    {
        var vm = _mapper.Map<EventDetailVm>(_event);

        Assert.Equal(_event.Id, vm.EventId);
        Assert.Equal(_event.Description, vm.Description);
        Assert.Equal(_event.CategoryId, vm.CategoryId);
    }

    [Fact]
    public void Event_To_EventExportDto_MapsIdToEventId()
    {
        Assert.Equal(_event.Id, _mapper.Map<EventExportDto>(_event).EventId);
    }

    [Fact]
    public void Category_To_CategoryListVm_And_CreateCategoryDto_MapIdToCategoryId()
    {
        Assert.Equal(_category.Id, _mapper.Map<CategoryListVm>(_category).CategoryId);
        Assert.Equal(_category.Id, _mapper.Map<CreateCategoryDto>(_category).CategoryId);
    }

    [Fact]
    public void Category_To_CategoryDto_MapsByConvention()
    {
        var dto = _mapper.Map<CategoryDto>(_category);

        Assert.Equal(_category.Id, dto.Id);
        Assert.Equal(_category.Name, dto.Name);
    }

    [Fact]
    public void CreateEventCommand_To_Event_MapsByConvention()
    {
        var command = new CreateEventCommand { Name = "Expo", Description = "d", Date = new DateTime(2027, 1, 1), CategoryId = Guid.NewGuid() };

        var entity = _mapper.Map<Event>(command);

        Assert.Equal(command.Name, entity.Name);
        Assert.Equal(command.Description, entity.Description);
        Assert.Equal(command.Date, entity.Date);
        Assert.Equal(command.CategoryId, entity.CategoryId);
    }

    [Fact]
    public void CreateEventCommand_ToString_DescribesTheEvent()
    {
        var command = new CreateEventCommand { Name = "Expo", Description = "d", Date = new DateTime(2027, 1, 1) };

        var text = command.ToString();

        Assert.StartsWith("Event name: Expo; On: ", text);
        Assert.EndsWith("; Description: d", text);
    }
}
