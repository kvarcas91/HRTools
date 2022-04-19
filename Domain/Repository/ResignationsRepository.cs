using Domain.Automation;
using Domain.DataValidation;
using Domain.DataValidation.Resignation;
using Domain.Models;
using Domain.Models.Resignations;
using Domain.Storage;
using Domain.Types;
using System;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public sealed class ResignationsRepository : BaseRepository
    {

        private readonly IDataValidation _validator;
        public ResignationsRepository()
        {
            _validator = new ResignationValidation();
        }
        public Task<Response> InsertAsync(ResignationEntity resignation)
        {

            var validationResponse = _validator.Validate(resignation);
            if (!validationResponse.Success) return Task.Run(() => validationResponse);

            var ttLink = resignation.TTLink.Split('/');
            var ttID = ttLink[ttLink.Length-1];

            var timeLine = new Timeline().Create(resignation.EmployeeID, TimelineOrigin.Resignations);
            timeLine.EventMessage = $"Resignation request has been submitted by {resignation.CreatedBy} due to '{resignation.ReasonForResignation}' (TT id: {ttID}). Last working day - {resignation.LastWorkingDay.ToString(DataStorage.ShortPreviewDateFormat)}";
            
            string tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";
            string query = $"INSERT INTO resignations {resignation.GetHeader()} VALUES {resignation.GetValues()};{tlQuery}";
            Automate(resignation, AutomationAction.OnIntake);
            return ExecuteAsync(query);
        }

        public Task<string> IsResignedAsync(string emplId)
        {
            string query = $"SELECT ttLink FROM resignations WHERE employeeID = '{emplId}'";
            return GetCachedScalarAsync<string>(query);
        }

        public Task<Response> CancelResignationAsync(string emplId)
        {
            var timeLine = new Timeline().Create(emplId, TimelineOrigin.Resignations);
            timeLine.EventMessage = $"Resignation has been cancelled by {Environment.UserName}";

            string tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";

            string query = $"DELETE FROM resignations WHERE employeeID = '{emplId}';{tlQuery}";
            Automate(new ResignationEntity { EmployeeID = emplId}, AutomationAction.OnUpdate);
            return ExecuteAsync(query);
        }

        private void Automate(ResignationEntity obj, AutomationAction action)
        {
            Task.Run(() =>
            {
                var automation = new ResignationsAutomation(this).SetData(null, obj);
                automation.Invoke(action);
            });

        }

    }
}
