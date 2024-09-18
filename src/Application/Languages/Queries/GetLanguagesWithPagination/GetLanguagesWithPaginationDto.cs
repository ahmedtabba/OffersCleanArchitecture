using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Countries.Queries.GetCountriesWithPagination;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithPagination;
public class GetLanguagesWithPaginationDto : LanguageBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Language, GetLanguagesWithPaginationDto>();
        }
    }
}

