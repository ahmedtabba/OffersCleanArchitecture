using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Countries.Commands.UpdateCountry;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Languages.Commands.UpdateLanguage;
public class UpdateLanguageCommandValidator :AbstractValidator<UpdateLanguageCommand>
{
    private readonly ILogger<UpdateLanguageCommandValidator> _logger;
    private readonly ILanguageRepository _languageRepository;

    public UpdateLanguageCommandValidator(ILogger<UpdateLanguageCommandValidator> logger,
                                          ILanguageRepository languageRepository)
    {
        _logger = logger;
        _languageRepository = languageRepository;

        RuleFor(l => l.Id)
           .NotEmpty().WithMessage("Language must has Id")
           .CustomAsync(async (name, context, cancellationToken) =>
           {
               if (!await IsLanguageExisted(context.InstanceToValidate))
               {
                   context.AddFailure("Update Language", "Language Id must be correct");
               }
           });

        RuleFor(l => l.Name)
           .NotEmpty().WithMessage("Language must has Name")
           .CustomAsync(async (name, context, cancellationToken) =>
           {
               if (!await BeUniqueName(context.InstanceToValidate))
               {
                   context.AddFailure("Name", "Name must be unique.");
               }
           });

        RuleFor(l=>l.Code)
            .MaximumLength(2).WithMessage("Maximum Length of code is 2 char.")

            .NotEmpty().WithMessage("Language must has Code")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await BeUniqueAndAcceptableCode(context.InstanceToValidate))
                {
                    context.AddFailure("Code", "Code must be unique and acceptable.");
                }
            });

        RuleFor(l => l.RTL)
            .NotNull().Must(x => x == true || x == false)
            .WithMessage("Language must be RTL or not");
    }

    public async Task<bool> IsLanguageExisted(UpdateLanguageCommand command)
    {
        return await _languageRepository.GetByIdAsync(command.Id) != null;
    }

    public async Task<bool> BeUniqueName(UpdateLanguageCommand command)
    {
        return !await _languageRepository.GetAll()
             .AnyAsync(l => l.Name == command.Name && l.Id != command.Id);
    }

    public async Task<bool> BeUniqueAndAcceptableCode(UpdateLanguageCommand command)
    {
        if (LanguageISO.AcceptableISOSet1.Contains(command.Code))
        {
            return !await _languageRepository.GetAll()
             .AnyAsync(l => l.Code == command.Code && l.Id != command.Id);
        }
        return false;
    }
}
