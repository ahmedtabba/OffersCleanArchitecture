using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Utilities;
public static class NullableValuesHelper
{
    public static Guid ConvertNullableGuid (Guid? value)
    {
        if (value != null)
        {
            if (value is { } x)
            {
                return x;
            }
            return Guid.Empty;
        }
        return Guid.Empty;
    }
    // TODO : test and use this method if works in PostHelper Class
    public static DateTime ConvertNullableDateTime(DateTime? value)
    {
        if (value != null)
        {
            if (value is { } x)
            {
                return x;
            }
            return DateTime.MinValue;
        }
        return DateTime.MinValue;
    }
}
