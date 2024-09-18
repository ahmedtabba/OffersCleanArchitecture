using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Offers.CleanArchitecture.Application.Common.GenericExtensions;
public static class GenericExtension // this extension dosn't work writ now
{
    public static T ToNotNullable<T>(this T? item)
    {
        if (item != null)
        {
            return item is { } x ? x : item;
        }
        else
            return item;
    }

    public static bool IsEntityFrameworkQueryable<T>(this IQueryable<T> queryable)
    {
        var provider = queryable.Provider;
        return provider is IAsyncQueryProvider;
    }
}
