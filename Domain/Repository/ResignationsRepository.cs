using Domain.DataValidation;
using Domain.Models;
using Domain.Models.Resignations;
using Domain.Storage;
using Domain.Types;
using System;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class ResignationsRepository : BaseRepository
    {

        private DataValidator _validator;
        public ResignationsRepository()
        {
            _validator = new ResignationValidator();
        }
        public Task<Response> InsertAsync(ResignationEntity resignation)
        {

            var validationResponse = _validator.Validate(resignation);
            if (!validationResponse.Success) return Task.Run(() => validationResponse);
           
            var ttLink = resignation.TTLink.Split('/');
            var ttID = ttLink[ttLink.Length-1];

            var timeLine = new Timeline
            {
                EmployeeID = resignation.EmployeeID,
                CreatedAt = DateTime.Now,
                CreatedBy = resignation.CreatedBy,
                EventMessage = $"Resignation request has been submitted by {resignation.CreatedBy} due to ''{resignation.ReasonForResignation}'' (TT id: {ttID}). Last working day - {resignation.LastWorkingDay.ToString(DataStorage.ShortPreviewDateFormat)}"
            };

            string tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";

            string query = $"INSERT INTO resignations {resignation.GetHeader()} VALUES {resignation.GetValues()};{tlQuery}";
            return ExecuteAsync(query);
        }

        public Task<string> IsResignedAsync(string emplId)
        {
            string query = $"SELECT ttLink FROM resignations WHERE employeeID = '{emplId}'";
            return GetCachedScalarAsync<string>(query);
        }

        public Task<Response> CancelResignationAsync(string emplId)
        {
            var timeLine = new Timeline
            {
                EmployeeID = emplId,
                CreatedAt = DateTime.Now,
                CreatedBy = Environment.UserName,
                EventMessage = $"Resignation has been cancelled by {Environment.UserName}"
            };

            string tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";

            string query = $"DELETE FROM resignations WHERE employeeID = '{emplId}';{tlQuery}";
            return ExecuteAsync(query);
        }

    }
}
