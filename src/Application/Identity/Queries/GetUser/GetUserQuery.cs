using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.UpdateUserCommand;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Identity.Queries.GetUser;
public class GetUserQuery : IRequest<IApplicationUser>
{
    public string userId { get; set; } = null!;
}
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, IApplicationUser>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly IIdentityService _identityService;
    private readonly ILogger<GetUserQueryHandler> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;

    public GetUserQueryHandler(IApplicationGroupManager applicationGroupManager,
                                IIdentityService identityService,
                                ILogger<GetUserQueryHandler> logger,
                                ICountryRepository countryRepository,
                                ILanguageRepository languageRepository)
    {
        _applicationGroupManager = applicationGroupManager;
        _identityService = identityService;
        _logger = logger;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;
    }
    public async Task<IApplicationUser> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.userId);
        await new UserMethodsHelper(_applicationGroupManager).FillApplicationGroupHelper(user.Id, user);
        var country = await _countryRepository.GetByIdAsync(Guid.Parse(user.CountryId));
        user.CountryName = country.Name;
        var language = await _languageRepository.GetByIdAsync(Guid.Parse(user.LanguageId));
        user.LanguageName = language.Name;

        return user;
    }
}
