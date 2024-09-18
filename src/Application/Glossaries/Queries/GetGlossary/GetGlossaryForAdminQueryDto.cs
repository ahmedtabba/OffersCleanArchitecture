using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;


namespace Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossary;
public class GetGlossaryForAdminQueryDto : GlossaryBaseDto
{
    [JsonPropertyName("localization")]
    public List<GetGlossaryLocalizationForAdminQueryDto> GlossaryLocalizationDtos { get; set; } = new List<GetGlossaryLocalizationForAdminQueryDto>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Glossary, GetGlossaryForAdminQueryDto>();

        }
    }
}
