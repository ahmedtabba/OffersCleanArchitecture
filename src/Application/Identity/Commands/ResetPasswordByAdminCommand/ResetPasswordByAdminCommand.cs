using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.ResetMyPasswordCommand;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Offers.CleanArchitecture.Application.Identity.Commands.ResetPasswordByAdminCommand;
public class ResetPasswordByAdminCommand : IRequest
{
    public string UserId { get; set; }
    public string Password { get; set; }
}

public class ResetPasswordByAdminCommandHandler : IRequestHandler<ResetPasswordByAdminCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ResetPasswordByAdminCommand> _logger;

    public ResetPasswordByAdminCommandHandler(IIdentityService identityService, ILogger<ResetPasswordByAdminCommand> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }
    public async Task Handle(ResetPasswordByAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.UserId);
        await _identityService.ResetPasswordAsync(request.UserId, request.Password);

    }
}
