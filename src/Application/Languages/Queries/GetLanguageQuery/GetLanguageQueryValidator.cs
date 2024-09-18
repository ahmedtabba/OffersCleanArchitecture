using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Languages.Commands.UpdateLanguage;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageQuery;
public class GetLanguageQueryValidator : AbstractValidator<GetLanguageQuery>
{
    private readonly ILogger<GetLanguageQueryValidator> _logger;
    private readonly ILanguageRepository _languageRepository;

    public GetLanguageQueryValidator(ILogger<GetLanguageQueryValidator> logger,
                                     ILanguageRepository languageRepository)
    {
        _logger = logger;
        _languageRepository = languageRepository;

        RuleFor(l => l.LanguageId)
           .NotEmpty().WithMessage("Language must has Id")
           .CustomAsync(async (name, context, cancellationToken) =>
           {
               if (!await IsLanguageExisted(context.InstanceToValidate))
               {
                   context.AddFailure("Get Language", "Language is not found");
               }
           });
    }

    public async Task<bool> IsLanguageExisted(GetLanguageQuery query)
    {
        return await _languageRepository.GetByIdAsync(query.LanguageId) != null;
    }
}
