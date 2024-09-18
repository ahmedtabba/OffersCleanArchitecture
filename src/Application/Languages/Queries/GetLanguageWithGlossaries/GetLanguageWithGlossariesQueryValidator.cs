using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageWithGlossaries;
public class GetLanguageWithGlossariesQueryValidator : AbstractValidator<GetLanguageWithGlossariesQuery>
{
    private readonly ILogger<GetLanguageWithGlossariesQueryValidator> _logger;
    private readonly ILanguageRepository _languageRepository;

    public GetLanguageWithGlossariesQueryValidator(ILogger<GetLanguageWithGlossariesQueryValidator> logger,
                                                   ILanguageRepository languageRepository)
    {
        _logger = logger;
        _languageRepository = languageRepository;

        RuleFor(l => l.LanguageCode)
           .NotEmpty().WithMessage("Language must has code")
           .CustomAsync(async (name, context, cancellationToken) =>
           {
               if (!await IsLanguageExisted(context.InstanceToValidate))
               {
                   context.AddFailure("Get Language", "Language is not found");
               }
           });
    }

    public async Task<bool> IsLanguageExisted(GetLanguageWithGlossariesQuery query)
    {
        return await _languageRepository.GetLanguageByCodeAsync(query.LanguageCode) != null;
    }
}
