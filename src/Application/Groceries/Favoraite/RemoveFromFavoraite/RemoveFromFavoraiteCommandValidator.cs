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

namespace Offers.CleanArchitecture.Application.Groceries.Favoraite.RemoveFromFavoraite;

public class RemoveFromFavoraiteCommandValidator : AbstractValidator<RemoveFromFavoraiteCommand>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<RemoveFromFavoraiteCommandValidator> _logger;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IUser _user;

    public RemoveFromFavoraiteCommandValidator(IGroceryRepository groceryRepository,
                                               ILogger<RemoveFromFavoraiteCommandValidator> logger,
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
                    context.AddFailure("Remove from Favoraite", "Grocery is not found");
                }
            })
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsFavoraiteGroceryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Remove from Favoraite", "Grocery is not already added");
                }
            });
    }

    public async Task<bool> IsGroceryExisted(RemoveFromFavoraiteCommand command)
    {
        var grocery = await _groceryRepository.GetByIdAsync(command.groceryId);
        if (grocery is null)
            return false;
        return true;
    }

    public async Task<bool> IsFavoraiteGroceryExisted(RemoveFromFavoraiteCommand command)
    {
        var favoraiteGroceryToDelete = await _favoraiteGroceryRepository.GetAllAsTracking()
                .FirstOrDefaultAsync(f => f.GroceryId == command.groceryId && f.UserId == _user.Id);
        if (favoraiteGroceryToDelete == null)
            return false;
        return true;

    }
}
