using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationGroup;
public class GetNotificationGroupQuery : IRequest<GetNotificationGroupDto>
{
    public Guid NotificationGroupId { get; set; }
}

public class GetNotificationGroupQueryHandler : IRequestHandler<GetNotificationGroupQuery, GetNotificationGroupDto>
{
    private readonly ILogger<GetNotificationGroupQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationGroupRepository _notificationGroupRepository;

    public GetNotificationGroupQueryHandler(ILogger<GetNotificationGroupQueryHandler> logger,
                                            IMapper mapper,
                                            INotificationGroupRepository notificationGroupRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _notificationGroupRepository = notificationGroupRepository;
    }

    public async Task<GetNotificationGroupDto> Handle(GetNotificationGroupQuery request, CancellationToken cancellationToken)
    {
        var notificationGroup = await _notificationGroupRepository.GetWithNotificationsByIdAsync(request.NotificationGroupId);
        var notificationGroupDto = _mapper.Map<GetNotificationGroupDto>(notificationGroup);
        return notificationGroupDto;
    }
}
