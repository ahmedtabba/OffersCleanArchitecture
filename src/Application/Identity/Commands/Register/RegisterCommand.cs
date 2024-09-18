using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.Login;

namespace Offers.CleanArchitecture.Application.Identity.Commands.Register;
public class RegisterCommand : IRequest<RegistrationResponse>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegistrationResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(IIdentityService identityService, ILogger<RegisterCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }
    public async Task<RegistrationResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registrationResponse = await _identityService.RegisterAsync(new RegistrationAppRequest
        {
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
        });
        return registrationResponse;
    }
}
