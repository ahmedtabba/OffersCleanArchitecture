using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostLocalizationFieldType;
public class GetPostLocalizationFieldTypeQuery : IRequest<List<PostLocalizationFieldTypeDto>>
{

}

public class GetPostLocalizationFieldTypeQueryHandler : IRequestHandler<GetPostLocalizationFieldTypeQuery, List<PostLocalizationFieldTypeDto>>
{
    private readonly ILogger<GetPostLocalizationFieldTypeQueryHandler> _logger;

    public GetPostLocalizationFieldTypeQueryHandler(ILogger<GetPostLocalizationFieldTypeQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<List<PostLocalizationFieldTypeDto>> Handle(GetPostLocalizationFieldTypeQuery request, CancellationToken cancellationToken)
    {
        //get Enum Values
        var fieldType = Enum.GetValues<PostLocalizationFieldType>();
        List<PostLocalizationFieldTypeDto> result = new List<PostLocalizationFieldTypeDto>();
        // fill dto result
        for (int i = 0; i < fieldType.Length; i++)
        {
            var dto = new PostLocalizationFieldTypeDto();
            dto.Key = fieldType[i].ToString();
            dto.Value = Convert.ToInt32(fieldType[i]);
            result.Add(dto);
        }
        return await Task.FromResult(result);
    }
}
