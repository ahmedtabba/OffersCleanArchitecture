using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Offers.CleanArchitecture.Application.Identity.Queries.GetUsersWithPagination;
public class GetUsersWithPaginationQueryValidator : AbstractValidator<GetUsersWithPaginationQuery>
{
    private readonly ILogger<GetUsersWithPaginationQueryValidator> _logger;

    public GetUsersWithPaginationQueryValidator(ILogger<GetUsersWithPaginationQueryValidator> logger)
    {
        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.");
        _logger = logger;
    }
}
