namespace Offers.CleanArchitecture.Api.NeededDto.Post;

public class GetPostsByFavoriteGroceriesWithPaginationQueryDto
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
    public int? PostFilter { get; set; }

}
