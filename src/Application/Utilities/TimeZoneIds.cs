using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Utilities;
public static class TimeZoneIdentifiers
{
    static readonly string[] TimeZoneIds = [
        "Turkey Standard Time",
        "Syria Standard Time",
        "Arabic Standard Time",
        "Egypt Standard Time",
        "UTC",
        "UTC+01",
        "UTC+02",
        "UTC+03",
        "UTC+04",
        "UTC+05",
        "UTC+06",
        "UTC-01",
        "UTC-02",
        "UTC-03",
        "UTC-04",
        "UTC-05",
        "UTC-06"
    ];

    public static string[] AcceptableTimeZoneIds { get; } = TimeZoneIds;

    public static string Turkey = "Turkey Standard Time";
    public static string Syria = "Syria Standard Time";
    public static string ArabicStandard = "Arabic Standard Time";
    public static string Egypt = "Egypt Standard Time";
    public static string UTC = "UTC";
    public static string UTC_P_1 = "UTC+01";
    public static string UTC_P_2 = "UTC+02";
    public static string UTC_P_3 = "UTC+03";
    public static string UTC_P_4 = "UTC+04";
    public static string UTC_P_5 = "UTC+05";
    public static string UTC_P_6 = "UTC+06";
    public static string UTC_M_1 = "UTC-01";
    public static string UTC_M_2 = "UTC-02";
    public static string UTC_M_3 = "UTC-03";
    public static string UTC_M_4 = "UTC-04";
    public static string UTC_M_5 = "UTC-05";
    public static string UTC_M_6 = "UTC-06";
}
