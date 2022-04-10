using Domain.Models;
using Domain.Models.AWAL;
using Domain.Models.Resignations;
using Domain.Types;

namespace Domain.Factory
{
    public class DataImportObjectFactory
    {
        public IDataImportObject Create(string[] fields, DataMap map)
        {
            switch (map.DataImportType)
            {
                case DataImportType.Roster:
                    return (IDataImportObject)new Roster().ReadFromCSV(fields, map);
                case DataImportType.Awal:
                    return (IDataImportObject)new AwalEntity().ReadFromCSV(fields, map);
                //case DataImportType.Resignations:
                //    return (IDataImportObject)new ResignationEntity().ReadFromCSV(fields, map);
                default:
                    return null;
            }
        }
    }
}
