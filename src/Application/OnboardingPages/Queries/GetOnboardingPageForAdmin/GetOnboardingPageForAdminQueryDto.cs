using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPageForAdmin;
public class GetOnboardingPageForAdminQueryDto : OnboardingPageBaseDto
{
    public List<OnboardingPageForAdminLocalizationDto> LocalizationDtos { get; set; } = new List<OnboardingPageForAdminLocalizationDto>();
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OnboardingPage, GetOnboardingPageForAdminQueryDto>();
        }
    }
}
