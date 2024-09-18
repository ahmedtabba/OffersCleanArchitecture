using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossary;
public class GetGlossaryLocalizationForAdminQueryDto : GlossaryLocalizationBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GlossaryLocalization, GetGlossaryLocalizationForAdminQueryDto>();
        }
    }
}
