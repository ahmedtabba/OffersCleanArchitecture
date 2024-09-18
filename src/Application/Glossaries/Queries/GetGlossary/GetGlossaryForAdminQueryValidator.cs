using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;

namespace Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossary;
public class GetGlossaryForAdminQueryValidator : AbstractValidator<GetGlossaryForAdminQuery>
{
    private readonly ILogger<GetGlossaryForAdminQueryValidator> _logger;
    private readonly IGlossaryRepository _glossaryRepository;

    public GetGlossaryForAdminQueryValidator(ILogger<GetGlossaryForAdminQueryValidator> logger,
                                             IGlossaryRepository glossaryRepository)
    {
        _logger = logger;
        _glossaryRepository = glossaryRepository;

        RuleFor(g => g.GlossaryId)
            .NotEmpty().WithMessage("Glossary must have Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGlossaryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("GlossaryId", "GlossaryId must be correct");
                }
            });

    }

    public async Task<bool> IsGlossaryExisted(GetGlossaryForAdminQuery query)
    {
        return await _glossaryRepository.GetByIdAsync(query.GlossaryId) != null;
    }
}
