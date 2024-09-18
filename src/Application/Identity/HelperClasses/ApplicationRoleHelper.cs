namespace Offers.CleanArchitecture.Application.Identity.HelperClasses;

public class ApplicationRoleHelper
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NormalizedName { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public string Description { get; set; } = null!;
}
