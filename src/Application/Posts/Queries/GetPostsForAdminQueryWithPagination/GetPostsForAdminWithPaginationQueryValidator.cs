using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsForAdminQueryWithPagination;
public class GetPostsForAdminWithPaginationQueryValidator : AbstractValidator<GetPostsForAdminWithPaginationQuery>
{
    private readonly ILogger<GetPostsForAdminWithPaginationQueryValidator> _logger;
    private readonly IGroceryRepository _groceryRepository;

    public GetPostsForAdminWithPaginationQueryValidator(ILogger<GetPostsForAdminWithPaginationQueryValidator> logger,
                                                        IGroceryRepository groceryRepository)
    {
        _logger = logger;
        _groceryRepository = groceryRepository;

        RuleFor(p => p.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(p => p.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.");

        RuleFor(p => p.GroceryId)
            .NotEmpty().WithMessage("Grocery Id should passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGroceryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("GroceryId", "GroceryId must be correct");
                }
            });
    }

    public async Task<bool> IsGroceryExisted(GetPostsForAdminWithPaginationQuery query)
    {
        return await _groceryRepository.GetByIdAsync(query.GroceryId) != null;
    }
}
