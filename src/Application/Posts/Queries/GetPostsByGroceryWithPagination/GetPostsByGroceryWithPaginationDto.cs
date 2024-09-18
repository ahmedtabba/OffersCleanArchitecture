using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
public class GetPostsByGroceryWithPaginationDto : PostBaseDto
{

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Post, GetPostsByGroceryWithPaginationDto>();
        }
    }
}
