using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;

namespace Offers.CleanArchitecture.Application.Groceries.Queries;
public class GroceryBaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string LogoPath { get; set; } = null!;
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = null!;
}
