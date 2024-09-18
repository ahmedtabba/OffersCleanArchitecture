using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public class BaseRepository<T> : IRepositoryAsync<T> where T : class
{
    protected readonly AppDbContext DbContext;


    public BaseRepository(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        return await DbContext.Set<T>().FindAsync(id);
    }

    public IQueryable<T> GetAll()
    {
       return DbContext.Set<T>().AsNoTracking().AsQueryable();
    }
    public IQueryable<T> GetAllAsTracking()
    {
        return DbContext.Set<T>().AsQueryable();
    }

    public async Task<T> AddAsync(T entity)
    {
        await DbContext.Set<T>().AddAsync(entity);
        //await DbContext.SaveChangesAsync();

        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        DbContext.Entry(entity).State = EntityState.Modified;
        //await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        DbContext.Set<T>().Remove(entity);
        //await DbContext.SaveChangesAsync();
    }

    //public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    //{
    //    return  await DbContext.SaveChangesAsync(cancellationToken);
    //}

    
}
