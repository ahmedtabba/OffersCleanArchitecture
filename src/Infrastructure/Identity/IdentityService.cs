using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Identity.Commands.ResetMyPasswordCommand;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Exceptions;
using System.Diagnostics.Metrics;
using System.Data.Common;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IOptions<JwtSettings> _jwtSettings1;
    private readonly IAuthorizationService _authorizationService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly IFileService _fileService;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly IUserNotificationGroupRepository _userNotificationGroupRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;

    //private readonly ILogger _logger;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(UserManager<ApplicationUser> userManager,
                           IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
                           IOptions<JwtSettings> jwtSettings,
                           IAuthorizationService authorizationService,
                           SignInManager<ApplicationUser> signInManager,
                           IApplicationGroupManager applicationGroupManager,
                           IFileService fileService,
                           INotificationGroupRepository notificationGroupRepository,
                           IUserNotificationGroupRepository userNotificationGroupRepository,
                           IUnitOfWorkAsync unitOfWork
                           //ILogger logger
        )
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _jwtSettings1 = jwtSettings;
        _authorizationService = authorizationService;
        _jwtSettings = jwtSettings.Value;
        _signInManager = signInManager;
        _applicationGroupManager = applicationGroupManager;
        _fileService = fileService;
        _notificationGroupRepository = notificationGroupRepository;
        _userNotificationGroupRepository = userNotificationGroupRepository;
        _unitOfWork = unitOfWork;
        //_logger = logger;
    }
 
    public async Task<(Result Result, string UserId)> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        // Add User
        //JobRole jobRole = request.JobRole ?? JobRole.None;
        var user = new ApplicationUser
        {
            Email = request.Email,
            FullName = request.FullName,
            UserName = request.UserName,
            CountryId = request.CountryId.ToString(),
            LanguageId = request.LanguageId.ToString(),
            PhoneNumber = request.PhoneNumber,
            JobRole = request.JobRole,
            PhotoURL = request.File != null ? await _fileService.UploadFileAsync(request.File) : "",
            HasPhoto = request.File != null ? true : false,
            NotificationGroupIds = request.NotificationGroupIds,
            TokenVersion = Guid.NewGuid().ToString(),
        };
        var addUserResponse = await _userManager.CreateAsync(user, request.Password);
        if (!addUserResponse.Succeeded)
        {
            return (addUserResponse.ToApplicationResult(), "");
        }

        // Join User With Groups
        await _applicationGroupManager.SetUserGroupsAsync(user.Id, request.GroupIds.ToArray());

        // Join User With Notification Groups
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            foreach (var notificationGroupId in request.NotificationGroupIds)
            {
                var notificationGroup = await _notificationGroupRepository.GetByIdAsync(Guid.Parse(notificationGroupId));
                UserNotificationGroup userNotificationGroup = new UserNotificationGroup
                {
                    NotificationGroup = notificationGroup,
                    UserId = user.Id
                };
                await _userNotificationGroupRepository.AddAsync(userNotificationGroup);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        

        return (IdentityResult.Success.ToApplicationResult(), user.Id);
    }
    


    
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }
    

    
    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId,CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        var result = await _userManager.DeleteAsync(user!);
        // Delete user notification groups of the user
        if (result.Succeeded)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var userNotificationGroups = await _userNotificationGroupRepository.GetAllAsTracking()
                    .Where(un => un.UserId == userId)
                    .ToListAsync();
                foreach (var userNotificationGroup in userNotificationGroups)
                {
                    //var userNotificationGroupToDelete = await _userNotificationGroupRepository.GetByIdAsync(userNotificationGroupId);
                    await _userNotificationGroupRepository.DeleteAsync(userNotificationGroup);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        return result.ToApplicationResult();
    }

  
    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
    {
        /* test successful
        Random rnd = new Random();
        var testRandomNum = rnd.Next(1,100);
        switch (testRandomNum)
        {
            case < 20: 
                throw new Exception("DB exception");

            case >= 20 and < 40 :
                throw new Exception("server exception");

            case >= 40 and < 90:
                throw new Exception("something exception");
            default:
                try
                {
                    var user = await _userManager.FindByEmailAsync(request.Email);

                    if (user == null)
                    {
                        throw new Exception($"User with {request.Email} not found.");
                    }

                    //var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, lockoutOnFailure: false);
                    var result = await _signInManager.PasswordSignInAsync(user!, request.Password, false, lockoutOnFailure: false);

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Credentials for '{request.Email} aren't valid'.");
                    }

                    JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

                    AuthenticationResponse response = new AuthenticationResponse
                    {
                        Id = user.Id,
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                        Email = user.Email!,
                        //UserName = user.UserName!
                    };

                    return response;
                }
                catch (Exception ex)
                {
                    throw;
                }

        }*/
        
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception($"User with {request.Email} not found.");
            }

            //var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, lockoutOnFailure: false);
            var result = await _signInManager.PasswordSignInAsync(user!, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new Exception($"Credentials for '{request.Email} aren't valid'.");
            }
            if(string.IsNullOrWhiteSpace(user.TokenVersion))
                user.TokenVersion = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

            AuthenticationResponse response = new AuthenticationResponse
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email!,
                //UserName = user.UserName!
            };

            return response;
        }
        catch (Exception ex)
        {
            throw;

            //throw new Exception("canNotLogin");
        }
    }


    
    public async Task<RegistrationResponse> RegisterAsync(RegistrationAppRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new Exception($"Email '{request.Email}' already exists.");
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return new RegistrationResponse() { UserId = user.Id };
        }
        else
        {
            throw new Exception($"{result.Errors}");
        }

    }
    
    private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        for (int i = 0; i < roles.Count; i++)
        {
            //roleClaims.Add(new Claim("roles", roles[i]));

            roleClaims.Add(new Claim(ClaimTypes.Role, roles[i]));// لماذا لا نستخدم هذه
            
        }

        var claims = new[]
        {
             // لماذا لا نستخدم هذه
            new Claim(ClaimTypes.Name, user.UserName),

            new Claim(ClaimTypes.SerialNumber, user.Id),

            new Claim(ClaimTypes.Email, user.Email),

            // this will be used by SignalR to identify the user it will send the notification to
            new Claim(ClaimTypes.NameIdentifier,user.Id),//JwtRegisteredClaimNames.Jti

            new Claim(ClaimTypes.MobilePhone,user.PhoneNumber ?? ""),
            new Claim(ClaimTypes.Locality,user.LanguageId.ToString()),
            new Claim(type:"jobrole",value:user.JobRole.ToString()),
            new Claim(type:"PhotoURL",value:user.PhotoURL?.ToString()??""),
            new Claim(type:"TokenVersion",value:user.TokenVersion??""),
            new Claim(type:"country_Id",value:user.CountryId??"")


            //new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //new Claim(JwtRegisteredClaimNames.Email, user.Email),
            //new Claim("user_id", user.Id)
            }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    public IQueryable<IApplicationUser> GetAllUsers()
    {
        var users =  _userManager.Users.AsQueryable();
        return users;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        return user?.UserName ?? "";
    }

    async Task<IApplicationUser?> IIdentityService.GetUserByIdAsync(string userId)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Result> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        //get user group Ids and JobRole to determine if we need to change TokenVersion
        var userGroup = await _applicationGroupManager.GetUserGroupsAsync(user.Id);
        var userGroupIds = userGroup.Select(g => g.Id);
        var oldJobRole = user.JobRole;

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.CountryId = request.CountryId.ToString();
        user.LanguageId = request.LanguageId.ToString();
        user.JobRole = request.JobRole;

        string oldPhotoUrl = user.PhotoURL;
        user.PhotoURL = request.File != null ? await _fileService.UploadFileAsync(request.File) : oldPhotoUrl;
        if (request.File != null)
            await _fileService.DeleteFileAsync(oldPhotoUrl);
        var updateUserInfoResult =  await _userManager.UpdateAsync(user);
        if (!updateUserInfoResult.Succeeded)
        {
            //TODO:  Logging errors and return message instead of Errors
            //_logger.LogError($"userManager => update user Fail for the userId : {user.Id}", res.Errors.ToString());
            throw new Exception("update fail");
        }

        // Update Groups of user
        var updateUserGroupResult =  await _applicationGroupManager.SetUserGroupsAsync(user.Id, request.GroupIds.ToArray());
        if (!updateUserGroupResult.Succeeded)
        {
            throw new Exception("update fail");
        }

        // check if we need to change TokenVersion
        var x = userGroupIds.Count() == request.GroupIds.Count;
        var allMatches = userGroupIds.All(id => request.GroupIds.Contains(id));
        if (oldJobRole != request.JobRole || !(allMatches && userGroupIds.Count() == request.GroupIds.Count))
        {
            user.TokenVersion = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);
        }

        //Update NotificationGroups 
        //delete old NotificationGroups
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var userNotificationGroups = await _userNotificationGroupRepository.GetAllAsTracking()
                .Where(un => un.UserId == user.Id)
                .ToListAsync();
            foreach (var userNotificationGroup in userNotificationGroups)
            {
                //var userNotificationGroupToDelete = await _userNotificationGroupRepository.GetByIdAsync(userNotificationGroupId);
                await _userNotificationGroupRepository.DeleteAsync(userNotificationGroup);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        //add new NotificationGroups
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            foreach (var notificationGroupId in request.NotificationGroupIds)
            {
                var notificationGroup = await _notificationGroupRepository.GetByIdAsync(Guid.Parse(notificationGroupId));
                UserNotificationGroup userNotificationGroup = new UserNotificationGroup
                {
                    NotificationGroup = notificationGroup,
                    UserId = user.Id
                };
                await _userNotificationGroupRepository.AddAsync(userNotificationGroup);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        return (IdentityResult.Success.ToApplicationResult());
    }

    public async Task<AuthenticationResponse> ResetPasswordAsync(string userId,string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await _userManager.ResetPasswordAsync(user,token, newPassword);

        if (!resetResult.Succeeded)
        {
            throw new Exception("reset password fail");
        }
        // change TokenVersion
        user.TokenVersion = Guid.NewGuid().ToString();
        await _userManager.UpdateAsync(user);

        JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

        AuthenticationResponse response = new AuthenticationResponse
        {
            Id = user.Id,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email!,
        };

        return response;

    }
}
