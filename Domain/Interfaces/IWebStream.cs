using CsvHelper;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IWebStream
    {
        IList<T> Get<T>(string url, Action<CsvReader> map);
    }
}
