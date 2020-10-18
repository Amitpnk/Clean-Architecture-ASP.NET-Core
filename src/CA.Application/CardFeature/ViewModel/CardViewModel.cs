using AutoMapper;
using CA.Application.Common.Mapping;
using CA.Domain.Entities;
using System;

namespace CA.Application.CardFeature.ViewModel
{
    public class CardViewModel : IMapFrom<Card>
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int SerialNumber { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public string Description { get; set; }
        public string Synonmys { get; set; }
        public string Meaning { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Card, CardViewModel>()
                .ForMember(destination => destination.Id, source => source.MapFrom(source => source.Id))
                .ForMember(destination => destination.Code,
                    source => source.MapFrom(source => $"BG {source.Chapter}.{source.Verse}"))
                .ForMember(destination => destination.Name,
                        source => source.MapFrom(source => $"Chapter {source.Chapter}, Verse {source.Verse}"))
              ;
        }

    }
}