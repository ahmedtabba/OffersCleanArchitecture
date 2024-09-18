using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Data;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public class PostLocalizationRepository : BaseRepository<PostLocalization>, IPostLocalizationRepository
{
    public PostLocalizationRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
