using Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class ResignationsRepository : BaseRepository
    {

        public Task<Response> InsertAsync()
        {
            string query = "";
            return ExecuteAsync(query);
        }

        public Task<int> IsResignedAsync(string emplId)
        {
            string query = $"SELECT COUNT(*) FROM resignations WHERE employeeID = '{emplId}'";
            return GetCachedScalarAsync<int>(query);
        }

    }
}
