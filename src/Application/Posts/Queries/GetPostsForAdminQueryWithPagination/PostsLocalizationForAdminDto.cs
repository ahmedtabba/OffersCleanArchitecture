﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsForAdminQueryWithPagination;
public class PostsLocalizationForAdminDto : PostLocalizationBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PostLocalization, PostsLocalizationForAdminDto>();
        }
    }
}
