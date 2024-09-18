using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotifications;
public class GetNotificationsQuery : IRequest<IEnumerable<GetNotificationsDto>>
{
}
public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<GetNotificationsDto>>
{
    private readonly ILogger<GetNotificationsQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsQueryHandler(ILogger<GetNotificationsQueryHandler> logger,
                                        IMapper mapper,
                                        INotificationRepository notificationRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<GetNotificationsDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = _notificationRepository.GetAll();
        var notificationsDtos = await _mapper.ProjectTo<GetNotificationsDto>(notifications).ToListAsync();
        return notificationsDtos;
    }
}
