using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroupsWithPagination;
public class GetGroupsWithPaginationQueryValidator : AbstractValidator<GetGroupsWithPaginationQuery>
{
    private readonly ILogger<GetGroupsWithPaginationQueryValidator> _logger;

    public GetGroupsWithPaginationQueryValidator(ILogger<GetGroupsWithPaginationQueryValidator> logger)
    {
        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.");
        _logger = logger;
    }
}
