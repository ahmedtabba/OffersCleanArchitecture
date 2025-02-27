﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Countries.Queries.GetCountriesWithPagination;
public class GetCountriesWithPaginationDto : CountryBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Country, GetCountriesWithPaginationDto>();
        }
    }
}


