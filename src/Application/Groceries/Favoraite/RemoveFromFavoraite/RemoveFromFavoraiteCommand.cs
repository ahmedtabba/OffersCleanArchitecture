using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Events.GroceryEvents;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Groceries.Favoraite.GetUserFavoraites;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Application.Groceries.Favoraite.RemoveFromFavoraite;
public class RemoveFromFavoraiteCommand : IRequest
{
    public Guid groceryId { get; set; }
}

public class RemoveFromFavoraiteCommandHandler : IRequestHandler<RemoveFromFavoraiteCommand>
{

    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUser _user;
    private readonly ILogger<RemoveFromFavoraiteCommandHandler> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public RemoveFromFavoraiteCommandHandler(IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                             IGroceryRepository groceryRepository,
                                             IUser user,
                                             ILogger<RemoveFromFavoraiteCommandHandler> logger,
                                             IUnitOfWorkAsync unitOfWork)
    {
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _groceryRepository = groceryRepository;
        _user = user;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveFromFavoraiteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var favoraiteGroceryToDelete = await _favoraiteGroceryRepository.GetAllAsTracking()
                .FirstOrDefaultAsync(f => f.GroceryId == request.groceryId && f.UserId == _user.Id);
            await _favoraiteGroceryRepository.DeleteAsync(favoraiteGroceryToDelete!);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}

