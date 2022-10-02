using AutoMapper;
using CleanArch.Application.Features.Categories.Commands.CreateCategory;
using CleanArch.Application.Features.Categories.Commands.UpdateCategory;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesList;
using CleanArch.Application.Features.Categories.Queries.GetCategoriesListWithEvents;
using CleanArch.Application.Features.Events.Commands.CreateEvent;
using CleanArch.Application.Features.Events.Commands.UpdateEvent;
using CleanArch.Application.Features.Events.Queries.GetEventDetail;
using CleanArch.Application.Features.Events.Queries.GetEventsExport;
using CleanArch.Application.Features.Events.Queries.GetEventsList;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // For events
            CreateMap<Event, EventListVm>()
                .ForMember(dest =>
                        dest.EventId,
                        opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
            CreateMap<Event, CreateEventCommand>().ReverseMap();
            CreateMap<Event, UpdateEventCommand>().ReverseMap();
            CreateMap<Event, EventDetailVm>()
                 .ForMember(dest =>
                        dest.EventId,
                        opt => opt.MapFrom(src => src.Id))
                 .ReverseMap();
            CreateMap<Event, CategoryEventDto>().ReverseMap();
            CreateMap<Event, EventExportDto>().ReverseMap();

            // For Categories
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryListVm>()
                   .ForMember(dest =>
                        dest.CategoryId,
                        opt => opt.MapFrom(src => src.Id));
            CreateMap<Category, CategoryEventListVm>()
                .ForMember(dest =>
                       dest.CategoryId,
                        opt => opt.MapFrom(src => src.Id));
            CreateMap<Category, CreateCategoryCommand>();
            CreateMap<Category, CreateCategoryDto>()
                  .ForMember(dest =>
                       dest.CategoryId,
                        opt => opt.MapFrom(src => src.Id));
            CreateMap<Category, UpdateCategoryCommand>().ReverseMap();
        }
    }
}