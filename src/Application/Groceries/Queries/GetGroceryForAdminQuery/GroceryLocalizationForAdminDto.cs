﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryForAdminQuery;
public class GroceryLocalizationForAdminDto : GroceryLocalizationBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GroceryLocalization, GroceryLocalizationForAdminDto>();
        }
    }
}


