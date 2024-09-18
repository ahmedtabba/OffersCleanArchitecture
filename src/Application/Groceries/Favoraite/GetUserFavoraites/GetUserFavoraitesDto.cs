using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Groceries.Queries;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Favoraite.GetUserFavoraites;
public class GetUserFavoraitesDto : GroceryBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Grocery, GetUserFavoraitesDto>();
        }
    }
}
