using System.Text.Json.Serialization;
using Offers.CleanArchitecture.Application.Countries.Queries.GetCountriesWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
public class GetGroceriesWithPaginationDto : GroceryBaseDto
{
    public bool IsFavorite { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Grocery, GetGroceriesWithPaginationDto>();

        }
    }
}



