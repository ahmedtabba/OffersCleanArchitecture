using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryQuery;
using Offers.CleanArchitecture.Application.Groceries.Favoraite.GetUserFavoraites;
using Microsoft.Extensions.Logging;

namespace Offers.CleanArchitecture.Application.Groceries.Favoraite.GetUserFavoraites;
public class GetUserFavoraitesWithPaginationQueryValidator : AbstractValidator<GetUserFavoraitesWithPaginationQuery>
{
    private readonly ILogger<GetUserFavoraitesWithPaginationQueryValidator> _logger;

    public GetUserFavoraitesWithPaginationQueryValidator(ILogger<GetUserFavoraitesWithPaginationQueryValidator> logger)
    {
        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                    .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
        _logger = logger;
    }
    
}
