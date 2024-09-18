using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossariesForAdminWithPagination;
public class GetGlossariesForAdminWithPaginationQueryValidator : AbstractValidator<GetGlossariesForAdminWithPaginationQuery>
{
    private readonly ILogger<GetGlossariesForAdminWithPaginationQueryValidator> _logger;

    public GetGlossariesForAdminWithPaginationQueryValidator(ILogger<GetGlossariesForAdminWithPaginationQueryValidator> logger)
    {
        _logger = logger;

        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}
