using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Commands.UpdateGrocery;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;


namespace Offers.CleanArchitecture.Application.Posts.Commands.UpdatePost;
public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<UpdatePostCommandValidator> _logger;
    private readonly ILanguageRepository _languageRepository;
    private readonly IPostLocalizationRepository _postLocalizationRepository;

    public UpdatePostCommandValidator(IPostRepository postRepository,
                                      ILogger<UpdatePostCommandValidator> logger,
                                      ILanguageRepository languageRepository,
                                      IPostLocalizationRepository postLocalizationRepository)
    {
        _postRepository = postRepository;
        _logger = logger;
        _languageRepository = languageRepository;
        _postLocalizationRepository = postLocalizationRepository;
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Post must have a title");

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("Description must have a description");


        RuleFor(p => p.IsActive)
            .Must(x => x == true || x == false || x == null)
            .WithMessage("Wrong input of IsActive value");

        RuleFor(p=>p.Id)
            .NotEmpty().WithMessage("Post must have Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsPostExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update Post", "PostId must be correct");
                }
            });

        RuleFor(p => p.PostLocalizations)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await ArePostLocalizationsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Post", "Post Localizations have invalid or empty values");
                }
            });

        RuleFor(p => p.PostLocalizationImages)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await ArePostLocalizationsImageLanguageValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Post", "Post Localizations Images have invalid language values");
                }
            })
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await ArePostLocalizationsImageHaveRepetedValues(context.InstanceToValidate))
                {
                    context.AddFailure("Update Post", "Post Localizations Images have repeted values");
                }
            });

        RuleFor(p => p.DeletedLocalizedAssetsIds)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreDeletedLocalizedImagesIdsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Post", "Old Post Localizations Images not found or not ImagePath type");
                }
            });
    }
    public async Task<bool> IsPostExisted(UpdatePostCommand command)
    {
        return await _postRepository.GetByIdAsync(command.Id) != null;
    }
    public async Task<bool> ArePostLocalizationsValid(UpdatePostCommand command)
    {
        List<Language> languages;
        var query = _languageRepository.GetAll();
        if (query.IsEntityFrameworkQueryable())
            languages = await query.ToListAsync();
        else
            languages = query.ToList();
        foreach (var postLocalization in command.PostLocalizations)
        {
            if (!languages.Any(l => l.Id == postLocalization.LanguageId))
            {
                return false;
            }
            if (postLocalization.Value == string.Empty)
            {
                return false;
            }
        }
        return true;
    }

    public async Task<bool> ArePostLocalizationsImageLanguageValid(UpdatePostCommand command)
    {
        List<Language> languages;
        var query = _languageRepository.GetAll();
        if (query.IsEntityFrameworkQueryable())
            languages = await query.ToListAsync();
        else
            languages = query.ToList();
        foreach (var postLocalizationImage in command.PostLocalizationImages)
        {
            if (!languages.Any(l => l.Id == postLocalizationImage.LanguageId))
            {
                return false;
            }
        }
        return true;
    }
    public async Task<bool> ArePostLocalizationsImageHaveRepetedValues(UpdatePostCommand command)
    {
        if (command.PostLocalizationImages.DistinctBy(pl => pl.LanguageId).Count() != command.PostLocalizationImages.Count)
        {
            return await Task.FromResult(false);
        }
        return await Task.FromResult(true);
    }

    public async Task<bool> AreDeletedLocalizedImagesIdsValid(UpdatePostCommand command)
    {
        foreach (var id in command.DeletedLocalizedAssetsIds)
        {
            var postLocalizedImage = await _postLocalizationRepository.GetByIdAsync(id);
            if (postLocalizedImage == null || postLocalizedImage.PostLocalizationFieldType !=(int)PostLocalizationFieldType.AssetPath)
                return false;
        }
        return true;
        
    }
}
