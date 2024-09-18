using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Identity.Commands.UpdateUserCommand;
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly IIdentityService _identityService;
    private readonly ILogger<UpdateUserCommandValidator> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly INotificationGroupRepository _notificationGroupRepository;

    public UpdateUserCommandValidator(IApplicationGroupManager applicationGroupManager,
                                      IIdentityService identityService,
                                      ILogger<UpdateUserCommandValidator> logger,
                                      ICountryRepository countryRepository,
                                      ILanguageRepository languageRepository,
                                      INotificationGroupRepository notificationGroupRepository)
    {
        _applicationGroupManager = applicationGroupManager;
        _identityService = identityService;
        _logger = logger;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;
        _notificationGroupRepository = notificationGroupRepository;
        RuleFor(u => u.UserId)
            .NotEmpty().WithMessage("User Id is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsUserExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update User", "User is not found");
                }
            });

        RuleFor(u => u.FullName)
            .NotEmpty().WithMessage("Full Name is required");

        RuleFor(u => u.PhoneNumber)
            .NotEmpty().WithMessage("Phone Number is required");

        RuleFor(u => u.GroupIds)
            //.NotEmpty().WithMessage("Group Ids are required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreGroupsExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update User", "Some Groups Ids are not valid");
                }
            });

        RuleFor(u => u.CountryId)
            .NotEmpty().WithMessage("Country Id is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsCountryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update User", "Country is not found");
                }
            });

        RuleFor(u => u.LanguageId)
            .NotEmpty().WithMessage("Language Id is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsLanguageExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update User", "Language is not found");
                }
            });

        RuleFor(u => u.NotificationGroupIds)
            .NotEmpty().WithMessage("NotificationGroup Ids is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreNotificationGroupsExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update User", "Some Notification Groups Ids are not valid");
                }
            });
    }

    public async Task<bool> AreGroupsExisted(UpdateUserCommand command)
    {
        // if we will set the user with no groups, we pass this validation
        if (command.GroupIds.Count == 0)
        {
            return true;
        }
        foreach (string groupId in command.GroupIds)
        {
            var group = await _applicationGroupManager.FindByIdAsync(groupId);
            if (group is null)
                return false;
        }
        return true;
    }

    public async Task<bool> IsUserExisted(UpdateUserCommand command)
    {
        var user = await _identityService.GetUserByIdAsync(command.UserId);
        if (user == null)
            return false;
        return true;
    }

    public async Task<bool> IsCountryExisted(UpdateUserCommand command)
    {
        var country = await _countryRepository.GetByIdAsync(command.CountryId);
        if (country is null)
            return false;
        return true;
    }

    public async Task<bool> IsLanguageExisted(UpdateUserCommand command)
    {
        var language = await _languageRepository.GetByIdAsync(command.LanguageId);
        if (language is null)
            return false;
        return true;
    }

    public async Task<bool> AreNotificationGroupsExisted(UpdateUserCommand command)
    {
        foreach (var notificationGroupId in command.NotificationGroupIds)
        {
            var notificationGroup = await _notificationGroupRepository.GetByIdAsync(Guid.Parse(notificationGroupId));
            if (notificationGroup is null)
                return false;
        }
        return true;
    }
}
