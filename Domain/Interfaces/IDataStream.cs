using Domain.Factory;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IDataStream
    {
        IList<IDataImportObject> Get(DataMap dataMap);
        void Write<T>(IEnumerable<T> dataList) where T : IWritable;
    }
}
