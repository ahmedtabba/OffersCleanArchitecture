using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Infrastructure.Identity;

namespace Offers.CleanArchitecture.Infrastructure.Services;
public class GroupStoreBase
{
    public DbContext Context
    {
        get;
        private set;
    }

    public DbSet<ApplicationGroup> DbEntitySet
    {
        get;
        private set;
    }

    public IQueryable<ApplicationGroup> EntitySet
    {
        get
        {
            return this.DbEntitySet;
        }
    }

    public GroupStoreBase(DbContext context)
    {
        this.Context = context;
        this.DbEntitySet = context.Set<ApplicationGroup>();
    }

    public void Create(ApplicationGroup entity)
    {
        this.DbEntitySet.Add(entity);
    }

    public void Delete(ApplicationGroup entity)
    {
        this.DbEntitySet.Remove(entity);
    }

    public async virtual Task<ApplicationGroup> GetByIdAsync(object id)
    {
        return await this.DbEntitySet.FindAsync(new object[] { id });
    }

    public async virtual Task<ApplicationGroup> GetByIdWithUsersndRolesAsync(object id)
    {
        return await this.DbEntitySet
            //.Include(g=>g.ApplicationUsers)
            //.Include(g=>g.ApplicationRoles)
            .FindAsync(new object[] { id });
    }


    public virtual ApplicationGroup GetById(object id)
    {
        return this.DbEntitySet.Include(c => c.ApplicationRoles).Include(c => c.ApplicationUsers).Single(c => c.Id == id.ToString());
    }


    public virtual void Update(ApplicationGroup entity)
    {
        if (entity != null)
        {
            this.Context.Entry<ApplicationGroup>(entity).State = EntityState.Modified;
        }
    }

}

