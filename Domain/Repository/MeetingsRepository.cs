using Domain.Factory;
using Domain.Models.Meetings;
using Domain.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class MeetingsRepository : BaseRepository
    {
        public Task<Response> InsertAllAsync(IList<IDataImportObject> awalList) => base.InsertAllAsync(awalList, "meetings");

        public Task<IEnumerable<MeetingsEntity>> GetEmployeeMeetingsAsync(string emplId) => GetCachedAsync<MeetingsEntity>($"SELECT * FROM meetings WHERE employeeID = '{emplId}' ORDER BY createdAt DESC");

    }
}
