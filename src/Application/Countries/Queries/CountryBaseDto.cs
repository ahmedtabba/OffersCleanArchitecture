using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Countries.Queries;
public class CountryBaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? FlagPath { get; set; } = null!;
    public string? TimeZoneId { get; set; } = null!;
    public string? Code { get; set; } = null!;

}
