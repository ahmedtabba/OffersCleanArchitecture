using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;

namespace Offers.CleanArchitecture.Application.Groceries.Commands.DeleteGrocery;
public class DeleteGroceryCommand : IRequest
{
    public Guid groceryId { get; set; }
}

public class DeleteGroceryCommandHandler : IRequestHandler<DeleteGroceryCommand>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<DeleteGroceryCommand> _logger;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public DeleteGroceryCommandHandler(IGroceryRepository groceryRepository,
                                       IFileService fileService,
                                       ILogger<DeleteGroceryCommand> logger,
                                       IUnitOfWorkAsync unitOfWork)
    {
        _groceryRepository = groceryRepository;
        _fileService = fileService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(DeleteGroceryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var grocery = await _groceryRepository.GetByIdAsync(request.groceryId);
            var groceryLogoPathToDelete = grocery.LogoPath;
            await _groceryRepository.DeleteAsync(grocery);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitAsync();
            await _fileService.DeleteFileAsync(groceryLogoPathToDelete);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
