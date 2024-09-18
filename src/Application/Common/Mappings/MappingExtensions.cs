using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Extensions;

namespace Offers.CleanArchitecture.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);

    //public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration) where TDestination : class
    //    => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync();
    public static Task<List<TDestination2>> ProjectToListAsync<TDestination2, TDestination1>(this IQueryable<TDestination1> queryable, IConfigurationProvider configuration) where TDestination1:class where TDestination2:class
    {
        if (queryable.IsEntityFrameworkQueryable())
            return queryable.ProjectTo<TDestination2>(configuration).ToListAsync();
        else
            return Task.FromResult(queryable.ProjectTo<TDestination2>(configuration).ToList());
    }



    public static IQueryable<TDestination> Order<TDestination>(this IQueryable<TDestination> queryable, string? sort) where TDestination : class
      => string.IsNullOrWhiteSpace(sort) ? queryable : queryable.OrderBy(sort);

}
