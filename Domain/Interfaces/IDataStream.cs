using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IDataStream
    {
        IList<T> Get<T>(string[] requiredHeaders, Func<string[], Dictionary<string, int>, T> createNewObj) where T : class, new();
        void Write<T>(IEnumerable<T> dataList) where T : IWritable;
    }
}
