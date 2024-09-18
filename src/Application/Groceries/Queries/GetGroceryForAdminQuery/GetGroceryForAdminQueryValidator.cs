using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryForAdminQuery;
public class GetGroceryForAdminQueryValidator : AbstractValidator<GetGroceryForAdminQuery>
{
    private readonly ILogger<GetGroceryForAdminQueryValidator> _logger;
    private readonly IGroceryRepository _groceryRepository;

    public GetGroceryForAdminQueryValidator(ILogger<GetGroceryForAdminQueryValidator> logger,
                                            IGroceryRepository groceryRepository)
    {
        _logger = logger;
        _groceryRepository = groceryRepository;

        RuleFor(g => g.GroceryId)
             .NotEmpty().WithMessage("Grocery Id should passed")
             .CustomAsync(async (name, context, cancellationToken) =>
             {
                 if (!await IsGroceryExisted(context.InstanceToValidate))
                 {
                     context.AddFailure("Get Grocery", "GroceryId must be correct");
                 }
             });
    }

    public async Task<bool> IsGroceryExisted(GetGroceryForAdminQuery query)
    {
        return await _groceryRepository.GetByIdAsync(query.GroceryId) != null;
    }
}
