using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Caching.Memory;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;

namespace Offers.CleanArchitecture.Infrastructure.Services;
public class CacheService : ICacheService // this service is responsible for dealing with memory (IMemoryCache) that asp.net core provide it
                                          // we use generic to save data in memory as the type we want
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    // get data from memory using its key
    public T GetData<T>(string key)
    {
        try
        {
            T item = (T) _memoryCache.Get(key);
            return item;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    // remove data from memory using its key 
    public void RemoveData(string key)
    {
        try
        {
            if (!string.IsNullOrEmpty(key))
            {
                _memoryCache.Remove(key);
            } 
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    // save data (value) to memory using its key and determined expirationTime of it
    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var result = true;
        try
        {
            if (!string.IsNullOrEmpty(key) && value != null)
            {
                _memoryCache.Set(key, value, expirationTime);
            }
            else
                result = false;
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
