using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroupsWithPagination;

namespace Offers.CleanArchitecture.Application.UserGroups.Queries.GetRoles;
public class GetRolesQuery : IRequest<IEnumerable<IApplicationRole>>
{

}
public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<IApplicationRole>>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<GetRolesQueryHandler> _logger;

    public GetRolesQueryHandler(IApplicationGroupManager applicationGroupManager, ILogger<GetRolesQueryHandler> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
    }
    public async Task<IEnumerable<IApplicationRole>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await _applicationGroupManager.GetAllRoles();
    }
}
