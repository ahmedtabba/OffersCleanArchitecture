using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Api.Utilities;

public static class UnAuthorizedUserCheeker
{
    private static IUserContext _userContext;
    public static IUserContext userContext {  get { return _userContext; } }
    public static (Guid CountryId, Guid LanguageId) GetCountryAndLanguageId(Guid? countryId, Guid? languageId, IUserContext userContext)
    {
        _userContext = userContext;
        Guid requestedCountryId;
        Guid requestedLanguageId;
        if (_userContext.CheckIfUserAuthorized())// get user language And country from token
        {
            requestedLanguageId = _userContext.GetLanguageIdOfUser();
            requestedCountryId = _userContext.GetCountryIdOfUser();
        }
        else
        {
            if (countryId is null || countryId is not Guid)
            {
                throw new ArgumentException("Country should be chosen");
            }
            // NullableValuesHelper.ConvertNullableGuid return Guid.Empty if Id is null
            requestedLanguageId = NullableValuesHelper.ConvertNullableGuid(languageId);
            requestedCountryId = NullableValuesHelper.ConvertNullableGuid(countryId);
        }
        return (requestedCountryId, requestedLanguageId);
    }
}
