using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries;
public class OnboardingPageBaseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string AssetPath { get; set; } = null!;
    public int Order { get; set; }
}
