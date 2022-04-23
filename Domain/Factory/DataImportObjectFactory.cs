using Domain.Models;
using Domain.Models.AWAL;
using Domain.Models.CustomMeetings;
using Domain.Models.Meetings;
using Domain.Models.Sanctions;
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
                case DataImportType.Meetings:
                        return (IDataImportObject)new MeetingsEntity().ReadFromCSV(fields, map);
                case DataImportType.CustomMeetings:
                    return (IDataImportObject)new CustomMeetingEntity().ReadFromCSV(fields, map);
                case DataImportType.Sanctions:
                    return (IDataImportObject)new SanctionEntity().ReadFromCSV(fields, map);
                default:
                    return null;
            }
        }
    }
}
