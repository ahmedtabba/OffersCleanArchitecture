using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Groceries.Favoraite.AddToFavoraite;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Favoraite.GetUserFavoraites;
public class GetUserFavoraitesWithPaginationQuery : IRequest<PaginatedList<GetUserFavoraitesDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    //public string? SearchText { get; set; }

}

public class GetUserFavoraitesWithPaginationQueryHandler : IRequestHandler<GetUserFavoraitesWithPaginationQuery, PaginatedList<GetUserFavoraitesDto>>
{
    private readonly IMapper _mapper;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IUser _user;
    private readonly ILogger<GetUserFavoraitesWithPaginationQueryHandler> _logger;

    public GetUserFavoraitesWithPaginationQueryHandler(IMapper mapper,
                                                       IGroceryRepository groceryRepository,
                                                       IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                                       IUser user,
                                                       ILogger<GetUserFavoraitesWithPaginationQueryHandler> logger)
    {
        _mapper = mapper;
        _groceryRepository = groceryRepository;
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _user = user;
        _logger = logger;
    }

    public async Task<PaginatedList<GetUserFavoraitesDto>> Handle(GetUserFavoraitesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var favoraiteGroceries = _favoraiteGroceryRepository.GetAll();
        //get favoraiteGroceries Of current user
        favoraiteGroceries = favoraiteGroceries.Where(fg => fg.UserId == _user.Id);
        // manipulation linq : FavoraiteGrocery => Grocery
        var groceriesOfUser = favoraiteGroceries.Select(x => new Grocery 
                                 {
                                     Id = x.Grocery.Id,
                                     Name = x.Grocery.Name,
                                     Description = x.Grocery.Description,
                                     Address = x.Grocery.Address,
                                     LogoPath = x.Grocery.LogoPath,
                                     Posts = x.Grocery.Posts,
                                 });
        //if (request.SearchText != null)
        //    query = query.Where(x => x.Grocery.Name.Contains(request.SearchText) || x.Grocery.Address.Contains(request.SearchText));

        var result = await groceriesOfUser
            .OrderBy(c => c.Name)
            .ProjectTo<GetUserFavoraitesDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}
