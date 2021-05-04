using CA.Application.CardFeature.ViewModel;
using MediatR;

namespace CA.Application.CardFeature.Commands
{
    public class CreateCardCommand : IRequest<CardViewModel>
    {
        public string Description { get; set; }
        public string Synonmys { get; set; }
        public string Meaning { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
    }
}
