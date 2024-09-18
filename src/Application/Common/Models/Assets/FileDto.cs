using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Models.Assets;
public class FileDto
{
    // Represent the logo or post information to be passed to application layer to be added
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public Stream Content { get; set; }
}
