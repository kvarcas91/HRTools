using Domain.Interfaces;

namespace Domain.Factory
{
    public interface IDataImportObject : IQueryable
    {
        object ReadFromCSV(string[] fields, DataMap dataMap);
    }
}
