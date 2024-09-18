using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Identity.Commands.CreateUser;
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private StringBuilder sb = new StringBuilder();
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly IIdentityService _identityService;
    private readonly IApplicationPasswordValidator _applicationPasswordValidator;
    private readonly ILogger<CreateUserCommandValidator> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly INotificationGroupRepository _notificationGroupRepository;

    public CreateUserCommandValidator(IApplicationGroupManager applicationGroupManager,
                                      IIdentityService identityService,
                                      IApplicationPasswordValidator applicationPasswordValidator,
                                      ILogger<CreateUserCommandValidator> logger,
                                      ICountryRepository countryRepository,
                                      ILanguageRepository languageRepository,
                                      INotificationGroupRepository notificationGroupRepository)
    {
        _applicationGroupManager = applicationGroupManager;
        _identityService = identityService;
        _applicationPasswordValidator = applicationPasswordValidator;
        _logger = logger;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;
        _notificationGroupRepository = notificationGroupRepository;
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsEmailAddressValid(context.InstanceToValidate))
                {
                    context.AddFailure("Create User", "Email address is already existed");
                }
            });

        RuleFor(u => u.UserName)
            .NotEmpty().WithMessage("User Name is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsUserNameValid(context.InstanceToValidate))
                {
                    context.AddFailure("Create User", "User Name is already existed");
                }
            });

        RuleFor(u => u.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsPasswordValid(context.InstanceToValidate))
                {
                    context.AddFailure("Password", sb.ToString());
                }
            });

        RuleFor(u => u.PhoneNumber)
            .NotEmpty().WithMessage("Phone Number is required");

        RuleFor(u=>u.FullName)
            .NotEmpty().WithMessage("Full Name is required");

        RuleFor(u=>u.GroupIds)
            .NotEmpty().WithMessage("Group Ids is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreGroupsExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Create User", "Some Groups Ids are not valid");
                }
            });

        RuleFor(u => u.CountryId)
            .NotEmpty().WithMessage("Country Id is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsCountryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Create User", "Country is not found");
                }
            });

        RuleFor(u=>u.LanguageId)
            .NotEmpty().WithMessage("Language Id is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsLanguageExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Create User", "Language is not found");
                }
            });

        RuleFor(u => u.NotificationGroupIds)
            .NotEmpty().WithMessage("NotificationGroup Ids is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreNotificationGroupsExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Create User", "Some Notification Groups Ids are not valid");
                }
            });
    }

    public async Task<bool> AreGroupsExisted(CreateUserCommand command)
    {
        foreach (string groupId in command.GroupIds)
        {
            var group = await _applicationGroupManager.FindByIdAsync(groupId);
            if (group is null)
                return false;
        }
        return true;
    }

    public async Task<bool> IsEmailAddressValid(CreateUserCommand command)
    {
        var user = await _identityService.GetUserByEmailAsync(command.Email);
        if (user != null)
            return false;
        return true;
    }

    public async Task<bool> IsUserNameValid(CreateUserCommand command)
    {
        var isUserNameExisted = await _identityService.GetAllUsers()
            .AnyAsync(u => u.UserName == command.UserName);
        if (isUserNameExisted)
            return false;
        return true;
    }

    public async Task<bool> IsPasswordValid(CreateUserCommand command)
    {
        var res = await _applicationPasswordValidator.ValidatePassword("", command.Password);
        if (!res.Succeeded)
        {
            foreach (string error in res.Errors)
            {
                sb.AppendLine(error);
            }
            return false;
        }
        return true;
    }

    public async Task<bool> IsCountryExisted(CreateUserCommand command)
    {
        var country = await _countryRepository.GetByIdAsync(command.CountryId);
        if (country is null)
            return false;
        return true;
    }

    public async Task<bool> IsLanguageExisted(CreateUserCommand command)
    {
        var language = await _languageRepository.GetByIdAsync(command.LanguageId);
        if (language is null)
            return false;
        return true;
    }

    public async Task<bool> AreNotificationGroupsExisted(CreateUserCommand command)
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
