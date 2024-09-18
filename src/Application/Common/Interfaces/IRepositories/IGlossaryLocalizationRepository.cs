using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;
using static System.Formats.Asn1.AsnWriter;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
public interface IGlossaryLocalizationRepository : IRepositoryAsync<GlossaryLocalization>
{
    public Task<List<GlossaryLocalization>> GetGlossaryLocalizationByGlossaryIdAsync(Guid glossaryId);
    public Task<List<GlossaryLocalization>> GetAllByLanguageIdAsync(Guid languageId);

}
