using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryForAdminQuery;
public class GetGroceryForAdminDto : GroceryBaseDto
{
    [JsonPropertyName("localization")]
    public List<GroceryLocalizationForAdminDto> GroceryLocalizationDtos { get; set; } = new List<GroceryLocalizationForAdminDto>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Grocery, GetGroceryForAdminDto>();
        }
    }
}


