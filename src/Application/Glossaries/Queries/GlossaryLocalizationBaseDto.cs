using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Glossaries.Queries;
public class GlossaryLocalizationBaseDto // represent the main information of Glossary Localization, and we add Key of Glossary and Language Name to response
{
    public Guid Id { get; set; }
    public Guid GlossaryId { get; set; }
    public string Key { get; set; } = null!;
    public Guid LanguageId { get; set; }
    public string LanguageName { get; set; } = null!;
    public string Value { get; set; } = null!;
}
