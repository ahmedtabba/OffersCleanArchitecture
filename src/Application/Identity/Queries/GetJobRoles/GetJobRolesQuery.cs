using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Identity.Queries.GetJobRoles;
public class GetJobRolesQuery : IRequest<List<JobRoleDto>>
{

}

public class GetJobRolesQueryHandler : IRequestHandler<GetJobRolesQuery, List<JobRoleDto>>
{
    private readonly ILogger<GetJobRolesQueryHandler> _logger;

    public GetJobRolesQueryHandler(ILogger<GetJobRolesQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<List<JobRoleDto>> Handle(GetJobRolesQuery request, CancellationToken cancellationToken)
    {
        //get Enum Values
        var jobRolesType = Enum.GetValues<JobRole>();
        List<JobRoleDto> result = new List<JobRoleDto>();
        // fill dto result
        for (int i = 0; i < jobRolesType.Length; i++)
        {
            var dto = new JobRoleDto();
            dto.Key = jobRolesType[i].ToString();
            dto.Value = Convert.ToInt32(jobRolesType[i]);
            result.Add(dto);
        }
        return await Task.FromResult(result);
    }
}
