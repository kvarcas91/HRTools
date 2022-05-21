using Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Data
{
    public class DataManager
    {
        public Task WriteToCsvAsync<T>(IDataStream dataStream, IEnumerable<T> rosterList) where T : IWritable
        {
            return Task.Run(() =>
            {
                dataStream.Write(rosterList);
            });
        }
    }
}
