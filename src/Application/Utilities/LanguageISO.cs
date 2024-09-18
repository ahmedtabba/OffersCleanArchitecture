using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Utilities;
public static class LanguageISO
{
    static readonly string[] ISOCode = [
        "ar",
        "en",
        "tr",
        "fr",
        "AR",
        "EN",
        "TR",
        "FR",
        "Ar",
        "En",
        "Tr",
        "Fr"
    ];

    public static string[] AcceptableISOSet1 { get; } = ISOCode;

    public static string TurkishCode = "tr";
    public static string ArabicCode = "ar";
    public static string EnglishCode = "en";
    public static string FrenchCode = "fr";
}
