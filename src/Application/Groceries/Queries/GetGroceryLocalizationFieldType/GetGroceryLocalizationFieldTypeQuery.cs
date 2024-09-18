using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryLocalizationFieldType;
public class GetGroceryLocalizationFieldTypeQuery : IRequest<List<GroceryLocalizationFieldTypeDto>>
{

}

public class GetGroceryLocalizationFieldTypeQueryHandler : IRequestHandler<GetGroceryLocalizationFieldTypeQuery, List<GroceryLocalizationFieldTypeDto>>
{
    private readonly ILogger<GetGroceryLocalizationFieldTypeQueryHandler> _logger;

    public GetGroceryLocalizationFieldTypeQueryHandler(ILogger<GetGroceryLocalizationFieldTypeQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<List<GroceryLocalizationFieldTypeDto>> Handle(GetGroceryLocalizationFieldTypeQuery request, CancellationToken cancellationToken)
    {
        //get Enum Values
        var fieldType = Enum.GetValues<GroceryLocalizationFieldType>();
        List<GroceryLocalizationFieldTypeDto> result = new List<GroceryLocalizationFieldTypeDto>();
        // fill dto result
        for (int i = 0; i < fieldType.Length; i++)
        {
            var dto = new GroceryLocalizationFieldTypeDto();
            dto.Key = fieldType[i].ToString();
            dto.Value = Convert.ToInt32(fieldType[i]);
            result.Add(dto);
        }
        return await Task.FromResult(result);
    }
}

