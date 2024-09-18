using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByFavoriteGroceriesWithPagination;
public class GetPostsByFavoriteGroceriesWithPaginationQueryValidator : AbstractValidator<GetPostsByFavoriteGroceriesWithPaginationQuery>
{
    private readonly ILogger<GetPostsByFavoriteGroceriesWithPaginationQueryValidator> _logger;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IUser _user;

    public GetPostsByFavoriteGroceriesWithPaginationQueryValidator(ILogger<GetPostsByFavoriteGroceriesWithPaginationQueryValidator> logger,
                                                                   IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                                                   IUser user)
    {

        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.")
                .CustomAsync(async (name, context, cancellationToken) =>
                {
                    if (!await AreThereFavoraiteGroceryForUser(context.InstanceToValidate))
                    {
                        context.AddFailure("Get Favoraite groceries posts", "there are no favoraite for user");
                    }
                });
        _logger = logger;
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _user = user;
    }
    public async Task<bool> AreThereFavoraiteGroceryForUser(GetPostsByFavoriteGroceriesWithPaginationQuery query)
    {
        return await _favoraiteGroceryRepository.CheckIfUserHasFavorate(_user.Id!);

    }
}
