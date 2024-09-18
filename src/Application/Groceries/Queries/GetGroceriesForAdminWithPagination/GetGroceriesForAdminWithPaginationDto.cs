using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesForAdminWithPagination;
public class GetGroceriesForAdminWithPaginationDto : GroceryBaseDto
{
    [JsonPropertyName("localization")]
    public List<GroceriesLocalizationForAdminDto> GroceryLocalizationDtos { get; set; } = new List<GroceriesLocalizationForAdminDto>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Grocery, GetGroceriesForAdminWithPaginationDto>();
        }
    }
}
