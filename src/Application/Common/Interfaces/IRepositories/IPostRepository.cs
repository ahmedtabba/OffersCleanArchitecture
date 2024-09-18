using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;
using static System.Formats.Asn1.AsnWriter;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
public interface IPostRepository : IRepositoryAsync<Post>
{
    public Task<Post?> GetPostWithGroceryByPostId(Guid id);// include its Grocery
    public IQueryable<Post> GetAllWithGroceries();

}
