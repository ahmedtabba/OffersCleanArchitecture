using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.ResetPasswordByAdminCommand;

namespace Offers.CleanArchitecture.Application.Identity.Commands.UpdateUserCommand;
public class UpdateUserCommand : IRequest
{
    public string UserId { get; set; } = null!;
    //public string Email { get; set; }
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
    public JobRole JobRole { get; set; }
    public FileDto? File { get; set; }
    public List<string> GroupIds { get; set; } = new List<string>();
    public List<string> NotificationGroupIds { get; set; } = new List<string>();

}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(IIdentityService identityService,
                                    IApplicationGroupManager applicationGroupManager,
                                    ILogger<UpdateUserCommandHandler> logger)
    {
        _identityService = identityService;
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
    }
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        //var user = await _identityService.GetUserByIdAsync(request.UserId);
        UpdateUserRequest userRequest = new UpdateUserRequest
        {
            UserId = request.UserId,
            FullName = request.FullName,
            CountryId = request.CountryId,
            LanguageId = request.LanguageId,
            File = request.File,
            PhoneNumber = request.PhoneNumber,
            JobRole = request.JobRole,
            GroupIds = request.GroupIds,
            NotificationGroupIds = request.NotificationGroupIds,
        };
        var result = await _identityService.UpdateUserAsync(userRequest,cancellationToken);
    }
}
