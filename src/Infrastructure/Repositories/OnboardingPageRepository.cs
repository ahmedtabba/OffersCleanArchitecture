using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Data;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public class OnboardingPageRepository : BaseRepository<OnboardingPage>, IOnboardingPageRepository
{
    public OnboardingPageRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

}
