using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Commands.DeleteNotificationGroup;
public class DeleteNotificationGroupCommand : IRequest
{
    public Guid NotificationGroupId { get; set; }
}

public class DeleteNotificationGroupCommandHandler : IRequestHandler<DeleteNotificationGroupCommand>
{
    private readonly ILogger<DeleteNotificationGroupCommandHandler> _logger;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public DeleteNotificationGroupCommandHandler(ILogger<DeleteNotificationGroupCommandHandler> logger,
                                                 INotificationGroupRepository notificationGroupRepository,
                                                 IUnitOfWorkAsync unitOfWork)
    {
        _logger = logger;
        _notificationGroupRepository = notificationGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteNotificationGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var notificationGroup = await _notificationGroupRepository.GetByIdAsync(request.NotificationGroupId);
            await _notificationGroupRepository.DeleteAsync(notificationGroup);
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
