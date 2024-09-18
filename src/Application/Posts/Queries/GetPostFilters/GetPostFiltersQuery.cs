using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Models.Enums;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostFilters;
public class GetPostFiltersQuery : IRequest<List<PostFilterDto>>
{

}

public class GetPostFiltersQueryHandler : IRequestHandler<GetPostFiltersQuery, List<PostFilterDto>>
{
    private readonly ILogger<GetPostFiltersQueryHandler> _logger;

    public GetPostFiltersQueryHandler(ILogger<GetPostFiltersQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<List<PostFilterDto>> Handle(GetPostFiltersQuery request, CancellationToken cancellationToken)
    {
        //get Enum Values
        var postFilters = Enum.GetValues<PostFilter>();
        List<PostFilterDto> result = new List<PostFilterDto>();
        // fill dto result
        for (int i = 0; i < postFilters.Length; i++)
        {
            var dto = new PostFilterDto();
            dto.Key = postFilters[i].ToString();
            dto.Value = Convert.ToInt32(postFilters[i]);
            result.Add(dto);
        }
        return await Task.FromResult(result);
    }
}
