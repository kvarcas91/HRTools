using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IWebStream
    {
        IList<T> Get<T>(string url, string[] requiredHeaders, Func<string[], Dictionary<string, int>, T> createNewObj) where T : class, new();
    }
}
