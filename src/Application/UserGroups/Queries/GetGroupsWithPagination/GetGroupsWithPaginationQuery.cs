using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroup;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroupsWithPagination;
public class GetGroupsWithPaginationQuery : IRequest<PaginatedList<IApplicationGroup>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
}

public class GetGroupsWithPaginationQueryHandler : IRequestHandler<GetGroupsWithPaginationQuery, PaginatedList<IApplicationGroup>>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<GetGroupsWithPaginationQueryHandler> _logger;

    public GetGroupsWithPaginationQueryHandler(IApplicationGroupManager applicationGroupManager, ILogger<GetGroupsWithPaginationQueryHandler> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
    }
    public async Task<PaginatedList<IApplicationGroup>> Handle(GetGroupsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var groups = _applicationGroupManager.GetAllGroups();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            groups = groups.Where(p => p.Name.ToLower().Contains(request.SearchText.ToLower()));
        }

        var result = await groups
            .OrderBy(p => p.Name)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        foreach (var group in result.Items)
        {
            await new GroupMethodsHelper(_applicationGroupManager).FillApplicationUsersHelper(group.Id, group);
            await new GroupMethodsHelper(_applicationGroupManager).FillApplicationRolesHelper(group.Id, group);
        }

        return result;
    }
}
