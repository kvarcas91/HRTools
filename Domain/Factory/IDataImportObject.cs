namespace Domain.Factory
{
    public interface IDataImportObject
    {
        object ReadFromCSV(string[] fields, DataMap dataMap);
    }
}
