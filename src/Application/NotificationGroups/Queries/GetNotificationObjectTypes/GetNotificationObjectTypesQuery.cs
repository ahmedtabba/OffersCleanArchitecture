
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationObjectTypes;
public class GetNotificationObjectTypesQuery :IRequest<List<NotificationObjectTypesDto>>
{

}

public class GetNotificationObjectTypesQueryHandler : IRequestHandler<GetNotificationObjectTypesQuery, List<NotificationObjectTypesDto>>
{
    private readonly ILogger<GetNotificationObjectTypesQueryHandler> _logger;

    public GetNotificationObjectTypesQueryHandler(ILogger<GetNotificationObjectTypesQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<List<NotificationObjectTypesDto>> Handle(GetNotificationObjectTypesQuery request, CancellationToken cancellationToken)
    {
        //get Enum Values
        var objectTypes = Enum.GetValues<NotificationObjectTypes>();
        List<NotificationObjectTypesDto> result = new List<NotificationObjectTypesDto>();
        // fill dto result
        for (int i = 0; i < objectTypes.Length; i++)
        {
            var dto = new NotificationObjectTypesDto();
            dto.Key = objectTypes[i].ToString();
            dto.Value = Convert.ToInt32(objectTypes[i]);
            result.Add(dto);
        }
        return await Task.FromResult(result);
    }
}
