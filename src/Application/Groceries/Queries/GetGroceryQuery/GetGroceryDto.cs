using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryQuery;
public class GetGroceryDto : GroceryBaseDto
{
    public ICollection<GetGroceryPostDto> Posts { get; set; } = new List<GetGroceryPostDto>();
    public bool IsFavorite { get; set; }

    public class Mapping : Profile
    {
    
        public Mapping()
        {
            CreateMap<Grocery, GetGroceryDto>();

        }
    }
}
