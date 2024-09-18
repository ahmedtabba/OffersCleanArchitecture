using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostFilters;
public class PostFilterDto
{
    public string Key { get; set; } = null!;
    public int Value { get; set; }
}
