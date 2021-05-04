using CA.Application.CardFeature.ViewModel;
using MediatR;
using System;

namespace CA.Application.CardFeature.Commands
{
    public class UpdateCardCommand : IRequest<CardViewModel>
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Synonmys { get; set; }
        public string Meaning { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
    }
}
