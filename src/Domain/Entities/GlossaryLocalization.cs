namespace Offers.CleanArchitecture.Domain.Entities;

public class GlossaryLocalization : BaseAuditableEntity
{
    public GlossaryLocalization() :base()
    {
        
    }
    public Guid GlossaryId { get; set; }
    public Guid LanguageId { get; set; }
    public string Value { get; set; } = null!;
}
