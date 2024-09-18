using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.Login;
using Offers.CleanArchitecture.Application.Posts.Commands.CreatePost;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsWithPagination;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.UserGroups.Commands.CreateGroup;
public class CreateGroupCommand : IRequest<string>
{ 
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<string> RolesIds { get; set;}

}

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, string>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<CreateGroupCommandHandler> _logger;

    public CreateGroupCommandHandler(IApplicationGroupManager applicationGroupManager,
                                     ILogger<CreateGroupCommandHandler> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
    }
    
    public async Task<string> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var groupToAddInfo = new CreateGroupRequest
        {
            Name =  request.Name,
            Description = request.Description
        };
        var idAdded = await _applicationGroupManager.CreateGroupAsync(groupToAddInfo);
        var group = await _applicationGroupManager.FindByIdAsync(idAdded);
        if (group != null)
        {
            await _applicationGroupManager.SetGroupRolesByRolesIdsAsync(idAdded, request.RolesIds.ToArray());
            return idAdded;
        }
        else
            throw new Exception("Group dosn't created succssesfuly");

    }
}


