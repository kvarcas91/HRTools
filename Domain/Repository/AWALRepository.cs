using Domain.DataValidation;
using Domain.DataValidation.AWAL;
using Domain.Factory;
using Domain.Models.AWAL;
using Domain.Types;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public sealed class AWALRepository : BaseRepository
    {
        private IDataValidation _validator;

        public AWALRepository()
        {
            _validator = new AwalValidation();
        }

        public Task<IEnumerable<AwalEntity>> GetEmployeeAwalAsync(string emplId)
        {
            string query = $"SELECT * FROM awal WHERE employeeID = '{emplId}' ORDER BY firstNCNSDate DESC;";
            return GetCachedAsync<AwalEntity>(query);
        }

        public Task<Response> InsertAllAsync(IList<IDataImportObject> awalList)
        {
            return InsertAllAsync(awalList, "awal");
        }

        public Task<Response> InsertAsync(AwalEntity awal)
        {
            var validationResponse = _validator.Validate(awal);
            if (!validationResponse.Success) return Task.Run(() => validationResponse);

            var query = $"INSERT INTO awal {awal.GetHeader()} VALUES {awal.GetValues()};";
            return ExecuteAsync(query);
           
        }

    }
}
