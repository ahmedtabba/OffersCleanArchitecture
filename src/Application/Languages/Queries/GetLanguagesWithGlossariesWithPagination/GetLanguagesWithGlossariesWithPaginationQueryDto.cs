using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithGlossariesWithPagination;
public class GetLanguagesWithGlossariesWithPaginationQueryDto : LanguageBaseDto
{
    public List<KeyValuePair<string,string>> Glossary { get; set; } = new List<KeyValuePair<string,string>>();
    //[JsonConverter(typeof(GlossaryConverter))]
    //public Dictionary<string, string> Glossary { get; set; } = new Dictionary<string, string>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Language, GetLanguagesWithGlossariesWithPaginationQueryDto>();
        }
    }
}

