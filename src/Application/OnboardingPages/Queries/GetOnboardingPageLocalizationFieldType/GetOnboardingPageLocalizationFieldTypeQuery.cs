using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPageLocalizationFieldType;
public class GetOnboardingPageLocalizationFieldTypeQuery : IRequest<List<OnboardingPageLocalizationFieldTypeDto>>
{

}

public class GetOnboardingPageLocalizationFieldTypeQueryHandler : IRequestHandler<GetOnboardingPageLocalizationFieldTypeQuery, List<OnboardingPageLocalizationFieldTypeDto>>
{
    private readonly ILogger<GetOnboardingPageLocalizationFieldTypeQueryHandler> _logger;

    public GetOnboardingPageLocalizationFieldTypeQueryHandler(ILogger<GetOnboardingPageLocalizationFieldTypeQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<List<OnboardingPageLocalizationFieldTypeDto>> Handle(GetOnboardingPageLocalizationFieldTypeQuery request, CancellationToken cancellationToken)
    {
        //get Enum Values
        var fieldTypes = Enum.GetValues<OnboardingPageLocalizationFieldType>();
        List<OnboardingPageLocalizationFieldTypeDto> result = new List<OnboardingPageLocalizationFieldTypeDto>();
        // fill dto result
        for (int i = 0; i < fieldTypes.Length; i++)
        {
            var dto = new OnboardingPageLocalizationFieldTypeDto();
            dto.Key = fieldTypes[i].ToString();
            dto.Value = Convert.ToInt32(fieldTypes[i]);
            result.Add(dto);
        }
        return await Task.FromResult(result);
    }
}
