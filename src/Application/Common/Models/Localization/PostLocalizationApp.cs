using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Common.Models.Localization;
public class PostLocalizationApp
{
    public Guid LanguageId { get; set; }
    public PostLocalizationFieldType FieldType { get; set; }
    public string Value { get; set; } = null!;
}
