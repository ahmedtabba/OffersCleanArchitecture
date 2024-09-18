using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Services;
public interface ISeedJobs// this interface is represent the jobs we want to add it to the back ground service according to code in application layer
                          
{
    // cache data in memory 
    public Task CacheGroceriesOfCountry(Guid countryId);
    public Task CacheGroceriesLocalization(Guid groceryId);
}
