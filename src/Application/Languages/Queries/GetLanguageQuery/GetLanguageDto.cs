using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageQuery;
public class GetLanguageDto : LanguageBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Language, GetLanguageDto>();
        }
    }
}
