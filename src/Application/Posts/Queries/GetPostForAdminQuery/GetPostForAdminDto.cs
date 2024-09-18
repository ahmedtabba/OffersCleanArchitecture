using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostForAdminQuery;
public class GetPostForAdminDto : PostBaseDto
{
    [JsonPropertyName("localization")]

    public List<PostLocalizationForAdminDto> PostLocalizationDtos { get; set; } = new List<PostLocalizationForAdminDto>();

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Post, GetPostForAdminDto>();
        }
    }

}
