using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageWithGlossaries;
public class GetLanguageWithGlossariesQueryDto : LanguageBaseDto
{
    public List<KeyValuePair<string, string>> Glossary { get; set; } = new List<KeyValuePair<string, string>>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Language, GetLanguageWithGlossariesQueryDto>();
        }
    }
}
