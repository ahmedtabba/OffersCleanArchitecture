using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;
using Offers.CleanArchitecture.Application.Groceries.Commands.DeleteGrocery;

namespace Offers.CleanArchitecture.Application.Groceries.Favoraite.AddToFavoraite;
public class AddToFavoraiteCommandValidator : AbstractValidator<AddToFavoraiteCommand>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<AddToFavoraiteCommandValidator> _logger;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IUser _user;

    public AddToFavoraiteCommandValidator(IGroceryRepository groceryRepository,
                                          ILogger<AddToFavoraiteCommandValidator> logger,
                                          IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                          IUser user)
    {
        _groceryRepository = groceryRepository;
        _logger = logger;
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _user = user;
        RuleFor(fg => fg.groceryId)
            .NotEmpty().WithMessage("Grocery Id must be passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGroceryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("AddToFavoraite", "Grocery is not found");
                }
            })
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (await IsGroceryAlreadyAdded(context.InstanceToValidate))
                {
                    context.AddFailure("AddToFavoraite", "Grocery is already added");
                }
            });
    }

    public async Task<bool> IsGroceryExisted(AddToFavoraiteCommand command)
    {
        var grocery = await _groceryRepository.GetByIdAsync(command.groceryId);
        if (grocery is null)
            return false;
        return true;

    }

    public async Task<bool> IsGroceryAlreadyAdded(AddToFavoraiteCommand command)
    {
        return await _favoraiteGroceryRepository.GetAll()
                .AnyAsync(f => f.UserId == _user.Id && f.GroceryId == command.groceryId);

    }
}
