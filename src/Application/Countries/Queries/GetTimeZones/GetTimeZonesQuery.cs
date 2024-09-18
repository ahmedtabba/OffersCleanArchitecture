using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Countries.Queries.GetTimeZones;
public class GetTimeZonesQuery : IRequest<PaginatedList<string>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
}

public class GetTimeZonesQueryHandler : IRequestHandler<GetTimeZonesQuery, PaginatedList<string>>
{
    private readonly ILogger<GetTimeZonesQueryHandler> _logger;

    public GetTimeZonesQueryHandler(ILogger<GetTimeZonesQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<PaginatedList<string>> Handle(GetTimeZonesQuery request, CancellationToken cancellationToken)
    {
        var timeZoneIdentifiers = TimeZoneIdentifiers.AcceptableTimeZoneIds.ToList();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
            timeZoneIdentifiers = timeZoneIdentifiers.Where(x => x.ToLower().Contains(request.SearchText.ToLower())).ToList();
        var timeZoneIdentifiersQueryable = timeZoneIdentifiers.AsQueryable();
        var result = await timeZoneIdentifiersQueryable.PaginatedListAsync(request.PageNumber,request.PageSize);
        return result;
    }
}
