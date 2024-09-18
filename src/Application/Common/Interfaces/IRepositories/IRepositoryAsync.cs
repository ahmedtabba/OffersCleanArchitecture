using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
public interface IRepositoryAsync<T> where T : class
{
    Task<T> GetByIdAsync(Guid id);
    IQueryable<T> GetAll();
    IQueryable<T> GetAllAsTracking();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    //Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
