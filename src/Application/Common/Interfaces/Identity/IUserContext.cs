using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
public interface IUserContext
{
    //bool IsAuthenticated { get; }

    //Guid UserId { get; }

    public Guid GetLanguageIdOfUser();
    public Guid GetCountryIdOfUser();
    public bool CheckIfUserAuthorized();
}

