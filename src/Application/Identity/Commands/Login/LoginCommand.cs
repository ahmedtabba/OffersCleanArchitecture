using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.DeleteUser;
using Offers.CleanArchitecture.Application.PollyAttributes;

namespace Offers.CleanArchitecture.Application.Identity.Commands.Login;
[RetryPolicy(RetryCount =5,SleepDuration =900)]
public class LoginCommand : IRequest<AuthenticationResponse>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IIdentityService identityService, ILogger<LoginCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<AuthenticationResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.AuthenticateAsync(new AuthenticationRequest
        {
            Email = request.Email,
            Password = request.Password
        });

        return result;
    }
}

