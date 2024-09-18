using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithPagination;
public class GetLanguagesWithPaginationQueryValidator : AbstractValidator<GetLanguagesWithPaginationQuary>
{
    private readonly ILogger<GetLanguagesWithPaginationQueryValidator> _logger;

    public GetLanguagesWithPaginationQueryValidator(ILogger<GetLanguagesWithPaginationQueryValidator> logger)
    {
        _logger = logger;

        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}
