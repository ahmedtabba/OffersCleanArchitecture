using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Identity.Commands.Login;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Offers.CleanArchitecture.Application.Identity.Commands.Register;
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly ILogger<RegisterCommandValidator> _logger;

    public RegisterCommandValidator(ILogger<RegisterCommandValidator> logger)
    {
        RuleFor(l => l.Email)
            .NotEmpty().WithMessage("Email cann't be empty")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(l => l.Password)
            .NotEmpty().WithMessage("Password cann't be empty")
            .Matches("^(?=.{8})(?=.*[^a-zA-Z])") // begining of line , contains at least 8 characters, ontains some non-letter character
            .Matches("^(?=.*[a-z])(?=.*[A-Z])"); // contains at least one uppercase letter and one lowercase letter
        _logger = logger;
    }
}
