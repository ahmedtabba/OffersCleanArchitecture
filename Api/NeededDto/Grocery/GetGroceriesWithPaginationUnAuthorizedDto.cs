namespace Offers.CleanArchitecture.Api.NeededDto.Grocery;

public class GetGroceriesWithPaginationUnAuthorizedDto
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
    public Guid? CountryId { get; set; }
    public Guid? LanguageId { get; set; }
}
