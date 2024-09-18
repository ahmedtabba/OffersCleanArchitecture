using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Identity;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
public interface IApplicationPasswordValidator
{
    public Task<Result> ValidatePassword(string userId, string password);
}
