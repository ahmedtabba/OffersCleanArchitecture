using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Glossaries.Queries;
public class GlossaryBaseDto // represent the main information of Glossary
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}
