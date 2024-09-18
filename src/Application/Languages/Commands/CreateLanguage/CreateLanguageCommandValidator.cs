using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Languages.Commands.CreateLanguage;
public class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
{
    private readonly ILogger<CreateLanguageCommandValidator> _logger;
    private readonly ILanguageRepository _languageRepository;

    public CreateLanguageCommandValidator(ILogger<CreateLanguageCommandValidator> logger,
                                         ILanguageRepository languageRepository)
    {
        _logger = logger;
        _languageRepository = languageRepository;

        RuleFor(l => l.Name)
            .MaximumLength(100).WithMessage("Maximum Length of name is 100 char.")

            .NotEmpty().WithMessage("Language must has Name")

            .MustAsync(BeUniqueName)
              .WithMessage("'{PropertyName}' must be unique.")
              .WithErrorCode("Unique");

        RuleFor(l => l.Code)
            .MaximumLength(2).WithMessage("Maximum Length of code is 2 char.")

            .NotEmpty().WithMessage("Language must has Code")

            .MustAsync(BeUniqueAndAcceptableCode)
              .WithMessage("'{PropertyName}' must be unique and acceptable.")
              .WithErrorCode("Unique_and_Acceptable");
    }

    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _languageRepository.GetAll()
            .AllAsync(l => l.Name != name, cancellationToken);
    }

    public async Task<bool> BeUniqueAndAcceptableCode(string code, CancellationToken cancellationToken)
    {
        if (LanguageISO.AcceptableISOSet1.Contains(code))
        {
            return await _languageRepository.GetAll()
            .AllAsync(l => l.Code != code, cancellationToken);
        }
        return false;
    }
}
