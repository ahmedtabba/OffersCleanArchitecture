using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Posts.Queries;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryQuery;
public class GetGroceryPostDto : PostBaseDto
{
    public class Mapping : Profile
    {

        public Mapping()
        {
            CreateMap<Post, GetGroceryPostDto>();

        }
    }
}
