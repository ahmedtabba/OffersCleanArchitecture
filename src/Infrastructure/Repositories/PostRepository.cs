using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Data;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public class PostRepository : BaseRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<Post> GetAllWithGroceries()
    {
        return DbContext.Posts
            .Include(p => p.Grocery)
            .AsQueryable();
    }

    public Task<Post?> GetPostWithGroceryByPostId(Guid id)
    {
        throw new NotImplementedException();
    }
}
