using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.ResetMyPasswordCommand;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

public interface IIdentityService
{

    IQueryable<IApplicationUser> GetAllUsers();
    Task<string?> GetUserNameAsync(string userId);
    Task<IApplicationUser?> GetUserByIdAsync(string userId);
    Task<IApplicationUser?> GetUserByEmailAsync(string email);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<RegistrationResponse> RegisterAsync(RegistrationAppRequest request);
    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken);
    Task<Result> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken);
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
    Task<AuthenticationResponse> ResetPasswordAsync(string userId, string newPassword);
}
