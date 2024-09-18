using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPageForAdmin;
public class OnboardingPageForAdminLocalizationDto : OnboardingPageLocalizationBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OnboardingPageLocalization, OnboardingPageForAdminLocalizationDto>();
        }
    }
}
