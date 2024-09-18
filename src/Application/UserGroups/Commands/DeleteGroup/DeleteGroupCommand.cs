using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.UserGroups.Commands.CreateGroup;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroup;

namespace Offers.CleanArchitecture.Application.UserGroups.Commands.DeleteGroup;
public class DeleteGroupCommand : IRequest
{
    public string groupId { get; set; } = null!;

}

public class GetGroupQueryHandler : IRequestHandler<DeleteGroupCommand>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<GetGroupQueryHandler> _logger;

    public GetGroupQueryHandler(IApplicationGroupManager applicationGroupManager, ILogger<GetGroupQueryHandler> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
    }
    public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        //var users = await _applicationGroupManager.GetGroupUsersAsync(request.groupId);
        //if (users.Any())
        //{
        //    throw new Exception("There are users depend on current group!!");
        //}
        var group = await _applicationGroupManager.FindByIdAsync(request.groupId); // group not null here
        var result = await _applicationGroupManager.DeleteGroupAsync(request.groupId);
    }
}
