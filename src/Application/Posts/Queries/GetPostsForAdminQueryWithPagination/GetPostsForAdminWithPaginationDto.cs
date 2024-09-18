using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsForAdminQueryWithPagination;
public class GetPostsForAdminWithPaginationDto : PostBaseDto
{
    [JsonPropertyName("localization")]

    public List<PostsLocalizationForAdminDto> PostLocalizationDtos { get; set; } = new List<PostsLocalizationForAdminDto>();

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Post, GetPostsForAdminWithPaginationDto>();
        }
    }
}
