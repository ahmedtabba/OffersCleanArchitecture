using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAsReadNotification;

namespace Offers.CleanArchitecture.Application.UserNotifications.Queries.GetAllUserNotifications;
public class GetAllUserNotificationsQuery : IRequest<PaginatedList<GetAllUserNotificationsDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public bool? IsRead { get; set; }
}

public class GetAllUserNotificationsQueryHandler : IRequestHandler<GetAllUserNotificationsQuery, PaginatedList<GetAllUserNotificationsDto>>
{
    private readonly ILogger<GetAllUserNotificationsQueryHandler> _logger;
    private readonly IUserNotificationService _userNotificationService;
    private readonly IUser _user;
    private readonly IMapper _mapper;

    public GetAllUserNotificationsQueryHandler(ILogger<GetAllUserNotificationsQueryHandler> logger,
                                                IUserNotificationService userNotificationService,
                                                IUser user,
                                                IMapper mapper)
    {
        _logger = logger;
        _userNotificationService = userNotificationService;
        _user = user;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetAllUserNotificationsDto>> Handle(GetAllUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = _userNotificationService.GetAllUserNotificationsQueryable(_user.Id, request.IsRead);

        var result = await notifications.
            OrderBy(n => n.NotificationDate)
            .ProjectTo<GetAllUserNotificationsDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        return result;
    }
}
