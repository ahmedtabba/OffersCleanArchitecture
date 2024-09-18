using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Posts.Commands.CreatePost;
public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<CreatePostCommandValidator> _logger;
    private readonly ILanguageRepository _languageRepository;

    public CreatePostCommandValidator(IPostRepository postRepository,
                                      IGroceryRepository groceryRepository,
                                      ILogger<CreatePostCommandValidator> logger,
                                      ILanguageRepository languageRepository)
    {
        _postRepository = postRepository;
        _groceryRepository = groceryRepository;
        _logger = logger;
        _languageRepository = languageRepository;
        RuleFor(p => p.Title)
             .NotEmpty().WithMessage("Post must has Tilte");

        RuleFor(p => p.Description)
             .NotEmpty().WithMessage("Post must has Description");

        RuleFor(p => p.IsActive)
            .Must(x => x == true || x == false || x == null)
            .WithMessage("Wrong input of IsActive value");

        RuleFor(p=>p.GroceryId)
            .NotEmpty().WithMessage("Post should attached to grocery")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGroceryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Post_Create", "Grocery is not found");
                }
            });
        RuleFor(p => p.File)
            .NotEmpty().WithMessage("Post must has Image");

        RuleFor(p => p.PostLocalizations)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await ArePostLocalizationsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Create Post", "Post Localizations have invalid or empty values");
                }
            });

        RuleFor(p => p.PostLocalizationImages)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await ArePostLocalizationsImageLanguageValid(context.InstanceToValidate))
                {
                    context.AddFailure("Create Post", "Post Localizations Images have invalid language values");
                }
            })
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await ArePostLocalizationsImageHaveRepeatedValues(context.InstanceToValidate))
                {
                    context.AddFailure("Create Post", "Post Localizations Images have repeated values");
                }
            });
    }

    public async Task<bool> IsGroceryExisted(CreatePostCommand command)
    {
        var grocery = await _groceryRepository.GetByIdAsync(command.GroceryId);
        if (grocery is null)
            return false;
        return true;

    }
    public async Task<bool> ArePostLocalizationsValid(CreatePostCommand command)
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

    public async Task<bool> ArePostLocalizationsImageLanguageValid(CreatePostCommand command)
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
    public async Task<bool> ArePostLocalizationsImageHaveRepeatedValues(CreatePostCommand command)
    {
        if (command.PostLocalizationImages.DistinctBy(pl => pl.LanguageId).Count() != command.PostLocalizationImages.Count)
        {
            return await Task.FromResult(false);
        }
        return await Task.FromResult(true);
    }
}
