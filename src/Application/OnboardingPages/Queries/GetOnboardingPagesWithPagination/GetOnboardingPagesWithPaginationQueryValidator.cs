using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPagesWithPagination;
public class GetOnboardingPagesWithPaginationQueryValidator : AbstractValidator<GetOnboardingPagesWithPaginationQuery>
{
    private readonly ILogger<GetOnboardingPagesWithPaginationQueryValidator> _logger;
    private readonly ILanguageRepository _languageRepository;

    public GetOnboardingPagesWithPaginationQueryValidator(ILogger<GetOnboardingPagesWithPaginationQueryValidator> logger,
                                                          ILanguageRepository languageRepository)
    {
        _logger = logger;
        _languageRepository = languageRepository;

        RuleFor(o => o.LanguageId)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsLanguageIdParamAcceptable(context.InstanceToValidate))
                {
                    context.AddFailure("LanguageId", "LanguageId must be correct");
                }
            });
    }

    public async Task<bool> IsLanguageIdParamAcceptable(GetOnboardingPagesWithPaginationQuery query)
    {
        // LanguageId is acceptable if it is null or valid
        if (query.LanguageId is null)
        {
            return true;
        }
        else
        {//check if LanguageId valid
            if (query.LanguageId != null)
            {//convert nullable to not nullable
                if (query.LanguageId is { } x)
                {
                    return await _languageRepository.GetByIdAsync(x) != null;
                }
                return true;
            }
            return true;
        }
    }
}
