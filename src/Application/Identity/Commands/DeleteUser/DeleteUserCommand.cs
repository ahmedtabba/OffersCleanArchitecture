using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.CreateUser;

namespace Offers.CleanArchitecture.Application.Identity.Commands.DeleteUser;
public class DeleteUserCommand : IRequest
{
    public string UserId { get; set; } = null!;
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(IIdentityService identityService,
                                    ILogger<DeleteUserCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeleteUserAsync(request.UserId,cancellationToken);
        if (!result.Succeeded)
        {
            //TODO : Logging Errors and return another message in the Exception
            throw new Exception(result.Errors.ToString());
        }
    }
}
