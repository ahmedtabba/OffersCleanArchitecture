using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.Register;

namespace Offers.CleanArchitecture.Application.Identity.Commands.ResetMyPasswordCommand;
public class ResetMyPasswordCommand : IRequest<AuthenticationResponse>
{
    public string UserId { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetMyPasswordCommand, AuthenticationResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(IIdentityService identityService, ILogger<ResetPasswordCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }
    public async Task<AuthenticationResponse> Handle(ResetMyPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(command.UserId);
        var result = await _identityService.ResetPasswordAsync(command.UserId,command.Password);
        return result;
    }
}
