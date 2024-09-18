using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Commands.UpdateNotificationGroup;
public class UpdateNotificationGroupCommand :IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Guid> NotificationsIds { get; set; } = new List<Guid>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateNotificationGroupCommand, NotificationGroup>();
        }
    }
}

public class UpdateNotificationGroupCommandHandler : IRequestHandler<UpdateNotificationGroupCommand>
{
    private readonly ILogger<UpdateNotificationGroupCommandHandler> _logger;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly INotificationRepository _notificationRepository;

    public UpdateNotificationGroupCommandHandler(ILogger<UpdateNotificationGroupCommandHandler> logger,
                                                 INotificationGroupRepository notificationGroupRepository,
                                                 IMapper mapper,
                                                 IUnitOfWorkAsync unitOfWork,
                                                 INotificationRepository notificationRepository)
    {
        _logger = logger;
        _notificationGroupRepository = notificationGroupRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(UpdateNotificationGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var existingNotificationGroup = await _notificationGroupRepository.GetWithNotificationsByIdAsync(request.Id);

            _mapper.Map(request, existingNotificationGroup);

            //delete old notifications
            existingNotificationGroup.Notifications.Clear();
            //add new notifications
            foreach (var notificationId in request.NotificationsIds)
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);
                existingNotificationGroup.Notifications.Add(notification);
            }
            await _notificationGroupRepository.UpdateAsync(existingNotificationGroup);
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
