using AutoMapper;
using CA.Application.Common.Mapping;
using CA.Domain.Entities;
using System;

namespace CA.Application.GroupFeature.ViewModel
{
    public class GroupViewModel : IMapFrom<Group>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Group, GroupViewModel>()
                .ForMember(destination => destination.Id, source => source.MapFrom(source => source.Id));
        }

    }
}