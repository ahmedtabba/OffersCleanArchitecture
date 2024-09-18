using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Common.Extensions;
public static class OrderByExtensions
{
    public static IQueryable<TSource> OrderBy<TSource>(
        this IQueryable<TSource> queryable,
        string sorts)
    {
        var sort = new SortCollection<TSource>(sorts);
        return sort.Apply(queryable);
    }

    public static IQueryable<TSource> OrderBy<TSource>(
        this IQueryable<TSource> queryable,
        params string[] sorts)
    {
        var sort = new SortCollection<TSource>(sorts);
        return sort.Apply(queryable);
    }

    public static IQueryable<TSource> OrderBy<TSource>(
        this IQueryable<TSource> queryable,
        string sorts,
        out SortCollection<TSource> sortCollection)
    {
        sortCollection = new SortCollection<TSource>(sorts);
        return sortCollection.Apply(queryable);
    }
}
