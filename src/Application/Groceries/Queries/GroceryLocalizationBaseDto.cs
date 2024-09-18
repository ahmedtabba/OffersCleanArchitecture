using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Groceries.Queries;
public class GroceryLocalizationBaseDto
{
    public Guid Id { get; set; }
    public Guid LanguageId { get; set; }
    public Guid GroceryId { get; set; }
    public int GroceryLocalizationFieldType { get; set; }
    public string Value { get; set; } = null!;
}
