using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Commands.UpdateGrocery;

namespace Offers.CleanArchitecture.Application.Groceries.Commands.DeleteGrocery;
public class DeleteGroceryCommandValidator : AbstractValidator<DeleteGroceryCommand>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<DeleteGroceryCommandValidator> _logger;

    public DeleteGroceryCommandValidator(IGroceryRepository groceryRepository,
                                         ILogger<DeleteGroceryCommandValidator> logger)
    {
        _groceryRepository = groceryRepository;
        _logger = logger;
        RuleFor(c => c.groceryId)

        .NotEmpty().WithMessage("Id Must be passed")
        .CustomAsync(async (name, context, cancellationToken) =>
        {
            if (!await CanDeleteGrocery(context.InstanceToValidate))
            {
                context.AddFailure("Delete Grocery", "Grocery is not found or has Posts!");
            }
        });
    }

    public async Task<bool> CanDeleteGrocery(DeleteGroceryCommand command)
    {
        // Check if Grocery is null or has posts
        var grocery = await _groceryRepository.GetGroceryWithPostsByGroceryId(command.groceryId);

        if (grocery is null || grocery.Posts.Any())
            return false;
        return true;
    }
}
