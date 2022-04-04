using Domain.Models.AWAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class AWALRepository : BaseRepository
    {

        public Task<IEnumerable<AwalEntity>> GetEmployeeAwalAsync(string emplId)
        {
            string query = $"SELECT * FROM awal WHERE employeeID = '{emplId}';";
            return GetCachedAsync<AwalEntity>(query);
        }

    }
}
