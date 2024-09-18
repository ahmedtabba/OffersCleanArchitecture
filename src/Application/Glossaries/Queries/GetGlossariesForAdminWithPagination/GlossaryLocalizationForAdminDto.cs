using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossariesForAdminWithPagination;
public class GlossaryLocalizationForAdminDto : GlossaryLocalizationBaseDto
{
    public class Mapping : Profile
    {
        public Mapping() 
        { 
            CreateMap<GlossaryLocalization, GlossaryLocalizationForAdminDto>();
        }
    }
}
