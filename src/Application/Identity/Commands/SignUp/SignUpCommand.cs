using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Application.Common.Models.Identity;

namespace Offers.CleanArchitecture.Application.Identity.Commands.SignUp;
public class SignUpCommand : IRequest<string>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmedPassword { get; set; } = null!;
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
  
}

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, string>
{
    private readonly ILogger<SignUpCommandHandler> _logger;
    private readonly IIdentityService _identityService;
    private readonly IApplicationGroupManager _applicationGroupManager;

    public SignUpCommandHandler(ILogger<SignUpCommandHandler> logger,
                                IIdentityService identityService,
                                IApplicationGroupManager applicationGroupManager)
    {
        _logger = logger;
        _identityService = identityService;
        _applicationGroupManager = applicationGroupManager;
    }

    public async Task<string> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var permissionGroup = await _applicationGroupManager.GetAllGroups().SingleOrDefaultAsync(g => g.Name == "Normal User Group");
        if (permissionGroup == null)
            throw new Exception("Normal User Group dosn't existed");

        CreateUserRequest createUserRequest = new CreateUserRequest
        {
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = "",
            FullName = request.Email,
            UserName = request.Email,
            CountryId = request.CountryId,
            LanguageId = request.LanguageId,
            JobRole = JobRole.User,
            GroupIds = [permissionGroup.Id],
        };
        var result = await _identityService.CreateUserAsync(createUserRequest, cancellationToken);
        if (!result.Result.Succeeded)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("Add user goes wrongly:");
            //foreach (var error in result.Result.Errors)
            //{
            //    sb.AppendLine(error);
            //}
            throw new Exception();
        }
        return result.UserId;
    }
}
