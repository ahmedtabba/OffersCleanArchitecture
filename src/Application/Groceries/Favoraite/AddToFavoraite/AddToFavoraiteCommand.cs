using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Commands.UpdateGrocery;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Favoraite.AddToFavoraite;
public class AddToFavoraiteCommand : IRequest
{
    public Guid groceryId { get; set; }
}

public class AddToFavoraiteCommandHandler : IRequestHandler<AddToFavoraiteCommand>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IUser _user;
    private readonly ILogger<AddToFavoraiteCommandHandler> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public AddToFavoraiteCommandHandler(IGroceryRepository groceryRepository,
                                       IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                       IUser user,
                                       ILogger<AddToFavoraiteCommandHandler> logger,
                                       IUnitOfWorkAsync unitOfWork)
    {
        _groceryRepository = groceryRepository;
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _user = user;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddToFavoraiteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            FavoraiteGrocery favoraiteGroceryToAdd = new FavoraiteGrocery();

            favoraiteGroceryToAdd.UserId = _user.Id;
            var grocery = await _groceryRepository.GetByIdAsync(request.groceryId);
            favoraiteGroceryToAdd.Grocery = grocery;
            await _favoraiteGroceryRepository.AddAsync(favoraiteGroceryToAdd);
            //grocery.FavoraiteGroceries.Add(favoraiteGroceryToAdd);
            //await _groceryRepository.UpdateAsync(grocery);
            //await _groceryRepository.SaveChangesAsync(cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw ;
        }
    }
}

