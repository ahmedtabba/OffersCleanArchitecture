using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
public interface ICountryRepository : IRepositoryAsync<Country>
{
    public Task<Country> GetCountryWithGroceriesByCountryIdAsync(Guid countryId);
    public Task<string> GetCountryNameByIdAsync(Guid countryId);
}
