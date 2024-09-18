using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Posts.Queries;
public class PostLocalizationBaseDto
{
    public Guid Id { get; set; }
    public Guid LanguageId { get; set; }
    public Guid PostId { get; set; }
    public int PostLocalizationFieldType { get; set; }
    public string Value { get; set; } = null!;
}
