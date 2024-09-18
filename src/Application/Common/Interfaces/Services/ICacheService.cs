using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Services;
public interface ICacheService// interface for caching service that we will use it to save and retrieve data to/from memory
{
    T GetData<T>(string key);
    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    void RemoveData(string key);
}

