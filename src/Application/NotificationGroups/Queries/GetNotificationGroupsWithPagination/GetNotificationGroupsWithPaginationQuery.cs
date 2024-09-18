using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationGroup;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationGroupsWithPagination;
public class GetNotificationGroupsWithPaginationQuery : IRequest<PaginatedList<GetNotificationGroupsWithPaginationDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
}

public class GetNotificationGroupsWithPaginationQueryHandler : IRequestHandler<GetNotificationGroupsWithPaginationQuery, PaginatedList<GetNotificationGroupsWithPaginationDto>>
{
    private readonly ILogger<GetNotificationGroupsWithPaginationQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly INotificationGroupRepository _notificationGroupRepository;

    public GetNotificationGroupsWithPaginationQueryHandler(ILogger<GetNotificationGroupsWithPaginationQueryHandler> logger,
                                                           IMapper mapper,
                                                           INotificationGroupRepository notificationGroupRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _notificationGroupRepository = notificationGroupRepository;
    }

    public async Task<PaginatedList<GetNotificationGroupsWithPaginationDto>> Handle(GetNotificationGroupsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var notificationGroups = _notificationGroupRepository.GetAll();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
            notificationGroups = notificationGroups.Where(x => x.Name.ToLower().Contains(request.SearchText.ToLower()));

        var result = await notificationGroups
            .OrderBy(n => n.Name)
            .ProjectTo<GetNotificationGroupsWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}
