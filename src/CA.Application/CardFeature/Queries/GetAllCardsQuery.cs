using CA.Application.CardFeature.ViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Application.CardFeature.Queries
{
    public class GetAllCardsQuery : IRequest<IEnumerable<CardViewModel>>
    {
        //    public int PageNumber { get; set; }
        //    public int PageSize { get; set; }
    }
}
