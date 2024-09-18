using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Offers.CleanArchitecture.Application.Identity.Commands.Login;
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly ILogger<LoginCommandValidator> _logger;

    public LoginCommandValidator(ILogger<LoginCommandValidator> logger)
    {
        RuleFor(l => l.Email)
            .NotEmpty().WithMessage("Email cann't be empty");

        RuleFor(l => l.Password)
            .NotEmpty().WithMessage("Password cann't be empty");
        _logger = logger;
    }
}
