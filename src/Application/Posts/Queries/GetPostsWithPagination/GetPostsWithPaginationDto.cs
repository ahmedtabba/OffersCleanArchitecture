using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsWithPagination;
public class GetPostsWithPaginationDto : PostBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Post, GetPostsWithPaginationDto>();
        }
    }
}
