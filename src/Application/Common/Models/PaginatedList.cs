using Offers.CleanArchitecture.Application.Common.GenericExtensions;

namespace Offers.CleanArchitecture.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.IsEntityFrameworkQueryable()? await source.CountAsync(): source.Count();
        // if pageSize = -1, here we will return all data without pagination 
        List<T> items;
      
        if (source.IsEntityFrameworkQueryable())
        items= pageSize != -1 
            ? await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
            : await source.ToListAsync();
        else
            items = pageSize != -1
               ?  source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
               :  source.ToList();

        // if we will return all data, TotalPages must be 1 in PaginatedList<T>
        if (pageSize == -1)
            pageSize = count;
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    
}
