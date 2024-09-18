using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Identity;

namespace Offers.CleanArchitecture.Infrastructure.Identity;
public class ApplicationPasswordValidator : IApplicationPasswordValidator
{
    //private readonly IPasswordValidator<IApplicationUser> _passwordValidator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PasswordValidator<ApplicationUser> _passwordValidator;

    public ApplicationPasswordValidator(/*IPasswordValidator<IApplicationUser> passwordValidator*/
                                        UserManager<ApplicationUser> userManager,
                                        PasswordValidator<ApplicationUser> passwordValidator)
    {
        //_passwordValidator = passwordValidator;
        _userManager = userManager;
        _passwordValidator = passwordValidator;
    }
    public async Task<Result> ValidatePassword(string userId,string password)
    {
        ApplicationUser user = new ApplicationUser();
        if (userId != "")
        { 
            user = await _userManager.FindByIdAsync(userId); 
        }
        var res = await _passwordValidator.ValidateAsync(_userManager, user, password);
        return res.ToApplicationResult();
    }
}
