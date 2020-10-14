using CA.Application.CardFeature.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.Queries
{
    public class GetCardByIdQuery : IRequest<CardViewModel>
    {
        public Guid CardId { get; set; }
        public GetCardByIdQuery(Guid id)
        {
            CardId = id;
        }
    }
}
