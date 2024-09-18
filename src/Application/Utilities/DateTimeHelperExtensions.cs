using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Utilities;
public static class DateTimeHelperExtensions
{
    //public static DateTime? ConvertSpecifiedDateTimeToUTC(DateTime? dateTime, TimeZoneInfo timeZoneInfo)
    //{
    //    if (dateTime != null)
    //    {
    //        // convert nullable PublishDate => not nullable
    //        if (dateTime is { } x)
    //        {
    //            // convert DateTime from timeZoneInfo that represent country TimeZone of the user to UTC DateTime
    //            var dt = DateTime.SpecifyKind(x, DateTimeKind.Unspecified);
    //            var sourceDateTime = TimeZoneInfo.ConvertTime(dt, timeZoneInfo, TimeZoneInfo.Utc);
    //            return DateTime.SpecifyKind(sourceDateTime, DateTimeKind.Utc);
    //        }
    //    }
    //    return dateTime;
    //}
    public static DateTime? ConvertSpecifiedDateTimeToUTC(this DateTime? dateTime, TimeZoneInfo timeZoneInfo)
    {
        if (dateTime != null)
        {
            // convert nullable PublishDate => not nullable
            if (dateTime is { } x)
            {
                // convert DateTime from timeZoneInfo that represent country TimeZone of the user to UTC DateTime
                var dt = DateTime.SpecifyKind(x, DateTimeKind.Unspecified);
                var sourceDateTime = TimeZoneInfo.ConvertTime(dt, timeZoneInfo, TimeZoneInfo.Utc);
                return DateTime.SpecifyKind(sourceDateTime, DateTimeKind.Utc);
            }
        }
        return dateTime;
    }
    //public static DateTime? ConvertSpecifiedDateTimeToTimeZoneDate(DateTime? dateTime, TimeZoneInfo timeZoneInfo)
    //{
    //    if (dateTime != null)
    //    {
    //        // convert nullable PublishDate => not nullable
    //        if (dateTime is { } x)
    //        {
    //            return TimeZoneInfo.ConvertTimeFromUtc(x, timeZoneInfo);
    //        }
    //    }
    //    return dateTime;
    //}
    public static DateTime? ConvertSpecifiedDateTimeToTimeZoneDate(this DateTime? dateTime, TimeZoneInfo timeZoneInfo)
    {
        if (dateTime != null)
        {
            // convert nullable PublishDate => not nullable
            if (dateTime is { } x)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(x, timeZoneInfo);
            }
        }
        return dateTime;
    }
}
