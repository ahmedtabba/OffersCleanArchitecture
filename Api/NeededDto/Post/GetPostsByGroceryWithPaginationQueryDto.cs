using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Api.NeededDto.Post;
public class GetPostsByGroceryWithPaginationQueryDto
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
    public Guid? CountryId { get; set; }
    public Guid? LanguageId { get; set; }
    public int? PostFilter {  get; set; }
}
