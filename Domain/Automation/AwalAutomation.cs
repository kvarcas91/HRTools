using Domain.Models;
using Domain.Models.AWAL;
using Domain.Models.Sanctions;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using System;
using System.Text;

namespace Domain.Automation
{
    internal sealed class AwalAutomation : ITaskAutomation<AwalEntity>
    {

        private AwalEntity _oldObj;
        private AwalEntity _newObj;

        private readonly BaseRepository  _repository;
        public AwalAutomation(BaseRepository repository)
        {
            _repository = repository;
        }

        public void Invoke(AutomationAction action)
        {
            switch (action)
            {
                case AutomationAction.OnIntake:
                    OnIntake();
                    return;
                case AutomationAction.OnUpdate:
                    OnUpdate();
                    return;
                default:
                    return;
            }
        }

        public ITaskAutomation<AwalEntity> SetData(AwalEntity oldObj, AwalEntity newObj)
        {
            _oldObj = oldObj;
            _newObj = newObj;
            return this;
        }

        private void OnIntake()
        {
            if (_newObj == null) return;
            var query = $"{GetMeetingsQuery()}";
            _repository.Execute(query);
        }

        private void OnUpdate()
        {
            if (_newObj == null || _oldObj == null) return;
            if (!string.IsNullOrEmpty(_newObj.Outcome) && !_newObj.Outcome.Equals(_oldObj.Outcome) && _newObj.AwalStatus == AwalStatus.Inactive)
            {
                _repository.Execute($"{GetSanctionQuery()}{GetTerminationQuery()}");
            }
        }

        private string GetSanctionQuery()
        {
            if (_newObj.Outcome.Equals("Termination")) return string.Empty;
            var stringBuilder = new StringBuilder();
            var sanction = new SanctionEntity().Init().SetEmployee(_newObj).SetSanction(_newObj.Outcome, _newObj.DisciplinaryDate);
            var timelineEntry = new Timeline().Create(sanction.EmployeeID, TimelineOrigin.Sanctions);
            timelineEntry.EventMessage = $"{sanction.Sanction} has been recorded by {Environment.UserName} and is active until {sanction.SanctionEndDate.ToString(DataStorage.ShortPreviewDateFormat)}";

            stringBuilder.Append($"INSERT INTO timeline {timelineEntry.GetHeader()} VALUES {timelineEntry.GetValues()};");
            stringBuilder.Append($@"UPDATE sanctions SET sanctionEndDate = '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}', overriden = '1', overridenBy = '{Environment.UserName}', overridenAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' 
                                                where employeeID = '{_newObj.EmployeeID}' AND sanctionEndDate > '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}' AND meetingType = '{(int)sanction.MeetingType}';");

            stringBuilder.Append($@"INSERT INTO sanctions {sanction.GetHeader()} VALUES {sanction.GetValues()};");
            return stringBuilder.ToString();
        }

        private string GetMeetingsQuery()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"UPDATE meetings SET meetingStatus = 'Pending', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE meetingStatus = 'Open' AND employeeID = '{_newObj.EmployeeID}';");
            stringBuilder.Append($"UPDATE custom_meetings SET meetingStatus = 'Pending', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE meetingStatus = 'Open' AND (claimantID = '{_newObj.EmployeeID}' OR respondentID = '{_newObj.EmployeeID}');");

            return stringBuilder.ToString();
        }
           

        private string GetTerminationQuery()
        {
            if (!_newObj.Outcome.Equals("Termination")) return string.Empty;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"UPDATE meetings SET meetingStatus = 'Cancelled', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE meetingStatus in ('Open', 'Pending') AND employeeID = '{_newObj.EmployeeID}';");
            stringBuilder.Append($"UPDATE custom_meetings SET meetingStatus = 'Cancelled', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE meetingStatus in ('Open', 'Pending') AND (claimantID = '{_newObj.EmployeeID}' OR respondentID = '{_newObj.EmployeeID}');");

            return stringBuilder.ToString();
        }
    }
}
