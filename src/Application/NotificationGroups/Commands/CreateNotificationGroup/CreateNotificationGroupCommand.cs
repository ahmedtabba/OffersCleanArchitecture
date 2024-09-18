using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Commands.CreateNotificationGroup;
public class CreateNotificationGroupCommand : IRequest<Guid>
{
    public string Name { get; set; } = null!;
    public List<Guid> NotificationsIds { get; set; } = new List<Guid>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateNotificationGroupCommand, NotificationGroup>();
        }
    }
}

public class AddNotificationGroupCommandHandler : IRequestHandler<CreateNotificationGroupCommand, Guid>
{
    private readonly ILogger<AddNotificationGroupCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    public AddNotificationGroupCommandHandler(ILogger<AddNotificationGroupCommandHandler> logger,
                                              IMapper mapper,
                                              INotificationGroupRepository notificationGroupRepository,
                                              INotificationRepository notificationRepository,
                                              IUnitOfWorkAsync unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _notificationGroupRepository = notificationGroupRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateNotificationGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var notificationGroupToAdd = _mapper.Map<NotificationGroup>(request);
            foreach (var notificationId in request.NotificationsIds)
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);
                notificationGroupToAdd.Notifications.Add(notification);
            }
            await _notificationGroupRepository.AddAsync(notificationGroupToAdd);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
            return notificationGroupToAdd.Id;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
