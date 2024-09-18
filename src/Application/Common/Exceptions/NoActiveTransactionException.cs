using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Exceptions;
public class NoActiveTransactionException:Exception
{
    public NoActiveTransactionException() : base("There is no active TANSACTION") { }
}
