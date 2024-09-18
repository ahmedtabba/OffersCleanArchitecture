using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Resources;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Glossaries.Commands.UpdateGlossary;
public class UpdateGlossaryCommandValidator : AbstractValidator<UpdateGlossaryCommand>
{
    private readonly ILogger<UpdateGlossaryCommandValidator> _logger;
    private readonly ILanguageRepository _languageRepository;
    private readonly IGlossaryRepository _glossaryRepository;

    public UpdateGlossaryCommandValidator(ILogger<UpdateGlossaryCommandValidator> logger,
                                          ILanguageRepository languageRepository,
                                          IGlossaryRepository glossaryRepository) 
    {
        _logger = logger;
        _languageRepository = languageRepository;
        _glossaryRepository = glossaryRepository;

        RuleFor(g => g.Id)
            .NotEmpty().WithMessage("Glossary must has Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGlossaryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update Glossary", "GlossaryId must be correct");
                }
            });

        RuleFor(g => g.Key)
            .NotEmpty().WithMessage("Glossary must has key")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await BeUniqueKey(context.InstanceToValidate))
                {
                    context.AddFailure("Update Glossary", "Glossary must has unique key");
                }
            });

        RuleFor(g => g.Value)
            .NotEmpty().WithMessage("Glossary must has value");

        RuleFor(g => g.GlossaryLocalizations)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGlossaryLocalizationsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Glossary", "Glossary Localizations has invalid or empty values");
                }
            });

    }

    public async Task<bool> IsGlossaryExisted(UpdateGlossaryCommand command)
    {
        return await _glossaryRepository.GetByIdAsync(command.Id) != null;
    }

    public async Task<bool> BeUniqueKey(UpdateGlossaryCommand command)
    {
        return !await _glossaryRepository.GetAll()
             .AnyAsync(l => l.Key == command.Key && l.Id != command.Id);
    }

    public async Task<bool> IsGlossaryLocalizationsValid(UpdateGlossaryCommand command)
    {
        List<Language> languages;
        var query = _languageRepository.GetAll();
        if (query.IsEntityFrameworkQueryable())
            languages = await query.ToListAsync();
        else
            languages = query.ToList();
        foreach (var glossaryLocalization in command.GlossaryLocalizations)
        {
            // language id must be correct and existed
            if (!languages.Any(l => l.Id == glossaryLocalization.LanguageId))
            {
                return false;
            }
            // value shouldn't be empty
            if (glossaryLocalization.Value == string.Empty)
            {
                return false;
            }
        }
        return true;
    }
}
