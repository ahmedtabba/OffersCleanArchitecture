using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Countries.Queries.GetCountryQuery;
public class GetCountryDto : CountryBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Country, GetCountryDto>();
        }
    }
}


