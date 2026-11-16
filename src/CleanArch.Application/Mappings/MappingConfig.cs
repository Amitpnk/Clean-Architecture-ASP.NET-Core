using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents;
using CleanArch.Application.Features.Events.Queries.GetEventDetail;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CleanArch.Application.Features.Events.Queries.GetEventsList;
using CleanArch.Domain.Entities;
using Mapster;

namespace CleanArch.Application.Mappings;

/// <summary>
/// Mapster configuration. Same-named members map by convention, so only the
/// renames (entity <c>Id</c> to <c>EventId</c>/<c>CategoryId</c>) are spelled out here.
/// </summary>
public sealed class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Events
        config.NewConfig<Event, EventListVm>()
            .Map(dest => dest.EventId, src => src.Id);
        config.NewConfig<Event, EventDetailVm>()
            .Map(dest => dest.EventId, src => src.Id);
        config.NewConfig<Event, CategoryEventDto>()
            .Map(dest => dest.EventId, src => src.Id);
        config.NewConfig<Event, EventExportDto>()
            .Map(dest => dest.EventId, src => src.Id);

        // Categories
        config.NewConfig<Category, CategoryListVm>()
            .Map(dest => dest.CategoryId, src => src.Id);
        config.NewConfig<Category, CategoryEventListVm>()
            .Map(dest => dest.CategoryId, src => src.Id);
        config.NewConfig<Category, CreateCategoryDto>()
            .Map(dest => dest.CategoryId, src => src.Id);

        // Command -> entity maps (CreateEventCommand, UpdateEventCommand, UpdateCategoryCommand)
        // are pure name-convention and need no configuration.
    }
}
