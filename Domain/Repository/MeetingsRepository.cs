using Domain.Factory;
using Domain.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class MeetingsRepository : BaseRepository
    {
        public Task<Response> InsertAllAsync(IList<IDataImportObject> awalList) => base.InsertAllAsync(awalList, "meetings");

    }
}
