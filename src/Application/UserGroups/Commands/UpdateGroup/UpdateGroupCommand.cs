using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;

namespace Offers.CleanArchitecture.Application.UserGroups.Commands.UpdateGroup;
public class UpdateGroupCommand : IRequest
{
    public string GroupId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Roles { get; set; }
}

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<UpdateGroupCommandHandler> _logger;

    public UpdateGroupCommandHandler(IApplicationGroupManager applicationGroupManager,ILogger<UpdateGroupCommandHandler> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
    }
    public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
       
            //var group = await _applicationGroupManager.FindByIdAsync(request.GroupId);
            //Guard.Against.NotFound(request.GroupId, group);
            // Update Group 
            UpdateGroupRequest groupRequest = new UpdateGroupRequest
            {
                Name = request.Name,
                GroupId = request.GroupId,
                Description = request.Description,
                Roles = request.Roles,
            };
            var result = await _applicationGroupManager.UpdateGroupAsync(groupRequest);
            if (!result.Succeeded)
                throw new Exception("Update Group dosn't go correctly");
        
    }
}


