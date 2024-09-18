using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Languages.Commands.DeleteLanguage;
public class DeleteLanguageCommandValidator :AbstractValidator<DeleteLanguageCommand>
{
    private readonly ILogger<DeleteLanguageCommandValidator> _logger;
    private readonly ILanguageRepository _languageRepository;
    private readonly IIdentityService _identityService;

    public DeleteLanguageCommandValidator(ILogger<DeleteLanguageCommandValidator> logger,
                                          ILanguageRepository languageRepository,
                                          IIdentityService identityService)
    {
        _logger = logger;
        _languageRepository = languageRepository;
        _identityService = identityService;
        RuleFor(l=>l.LanguageId)
            .NotEmpty().WithMessage("Id Must be passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await CanDeleteLanguage(context.InstanceToValidate))
                {
                    context.AddFailure("Delete Language", "Language is not found or there are users with this language!");
                }
            });
    }

    public async Task<bool> CanDeleteLanguage(DeleteLanguageCommand command)
    {
        var language = await _languageRepository.GetByIdAsync(command.LanguageId);
        //TODO : more validation when add relations with users and groceries and posts
        if (language is null) 
        {
            return false;
        }
        else
        {
            var areThereUsersOfLanguage = await _identityService.GetAllUsers()
                .AnyAsync(u => u.LanguageId == command.LanguageId.ToString());
            if (areThereUsersOfLanguage)
                return false;
        }
        return true;
    }
}
