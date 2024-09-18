using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.Queries.GetUser;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Identity.Queries.GetUsersWithPagination;

public class GetUsersWithPaginationQuery : IRequest<PaginatedList<IApplicationUser>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }
}

public class GetUsersWithPaginationQueryHandler : IRequestHandler<GetUsersWithPaginationQuery, PaginatedList<IApplicationUser>>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly IIdentityService _identityService;
    private readonly ILogger<GetUsersWithPaginationQueryHandler> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;

    public GetUsersWithPaginationQueryHandler(IApplicationGroupManager applicationGroupManager,
                                              IIdentityService identityService,
                                              ILogger<GetUsersWithPaginationQueryHandler> logger,
                                              ICountryRepository countryRepository,
                                              ILanguageRepository languageRepository)
    {
        _applicationGroupManager = applicationGroupManager;
        _identityService = identityService;
        _logger = logger;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;
    }

    public async Task<PaginatedList<IApplicationUser>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var users = _identityService
            .GetAllUsers();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
            users = users.Where(u => u.UserName.ToLower().Contains(request.SearchText.ToLower()) || u.Email.ToLower().Contains(request.SearchText.ToLower()));
        var result = await users.PaginatedListAsync(request.PageNumber, request.PageSize);
        foreach (var user in result.Items)
        {
            //Get Groups and fill this property
            //user.UserGroups  
            await new UserMethodsHelper(_applicationGroupManager).FillApplicationGroupHelper(user.Id, user);
            var country = await _countryRepository.GetByIdAsync(Guid.Parse(user.CountryId));
            user.CountryName = country.Name;
            var language = await _languageRepository.GetByIdAsync(Guid.Parse(user.LanguageId));
            user.LanguageName = language.Name;
        }
        return result;
    }
}

