using CA.Application.CardFeature.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.Commands
{
    public class CreateCardCommand : IRequest<CardViewModel>
    {
        //public Guid Id { get; set; }
        //public string Code { get; set; }
        //public string Name { get; set; }
        public string Description { get; set; }
        public string Synonmys { get; set; }
        public string Meaning { get; set; }
        public int Chapter { get; set; }
        public int Verse { get; set; }
    }
}
