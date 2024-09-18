using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;


namespace Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossariesForAdminWithPagination;
public class GetGlossariesForAdminWithPaginationQueryDto : GlossaryBaseDto
{
    [JsonPropertyName("localization")]
    public List<GlossaryLocalizationForAdminDto> GlossaryLocalizationDtos { get; set; } = new List<GlossaryLocalizationForAdminDto>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Glossary, GetGlossariesForAdminWithPaginationQueryDto>();

        }
    }
}
