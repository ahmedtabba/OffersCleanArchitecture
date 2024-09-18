using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithGlossariesWithPagination;
using Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageWithGlossaries;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Utilities;
public static class JsonBuilder
{
    public static string BuildLanguagesGlossariesJson(PaginatedList<GetLanguagesWithGlossariesWithPaginationQueryDto> paginatedLanguages)
    {
        List<GetLanguagesWithGlossariesWithPaginationQueryDto> languages = paginatedLanguages.Items.ToList();
        var sb = new StringBuilder();
        sb.AppendLine("[");

        for (int i = 0; i < languages.Count; i++)
        {
            var language = languages[i];
            sb.AppendLine("  {");

            sb.AppendLine("    \"glossary\": {");
            foreach (var kvp in language.Glossary)
            {
                sb.AppendFormat("      \"{0}\": \"{1}\"", kvp.Key, kvp.Value);
                sb.AppendLine(",");
            }
            if (language.Glossary.Count > 0)
                sb.Length--; // Remove the last comma
            sb.AppendLine("    },");

            sb.AppendFormat("    \"id\": \"{0}\",", language.Id);
            sb.AppendLine();
            sb.AppendFormat("    \"name\": \"{0}\",", language.Name);
            sb.AppendLine();
            sb.AppendFormat("    \"code\": \"{0}\",", language.Code);
            sb.AppendLine();
            sb.AppendFormat("    \"rtl\": {0}", language.RTL.ToString().ToLower());
            sb.AppendLine();

            sb.Append("  }");
            if (i < languages.Count - 1)
            {
                sb.Append(",");
            }
            sb.AppendLine();
        }

        sb.AppendLine("]");

        return sb.ToString();
    }


    public static string BuildLanguagesGlossariesJson(GetLanguageWithGlossariesQueryDto language)
    {
        var sb = new StringBuilder();

        sb.AppendLine("{");

        sb.AppendLine("    \"glossary\": {");
        foreach (var kvp in language.Glossary)
        {
            sb.AppendFormat("      \"{0}\": \"{1}\"", kvp.Key, kvp.Value);
            sb.AppendLine(",");
        }
        if (language.Glossary.Count > 0)
            sb.Length--; // Remove the last comma
        sb.AppendLine("    },");

        sb.AppendFormat("    \"id\": \"{0}\",", language.Id);
        sb.AppendLine();
        sb.AppendFormat("    \"name\": \"{0}\",", language.Name);
        sb.AppendLine();
        sb.AppendFormat("    \"code\": \"{0}\",", language.Code);
        sb.AppendLine();
        sb.AppendFormat("    \"rtl\": {0}", language.RTL.ToString().ToLower());
        sb.AppendLine();

        sb.Append("}");
        sb.AppendLine();

        return sb.ToString();
    }
}
