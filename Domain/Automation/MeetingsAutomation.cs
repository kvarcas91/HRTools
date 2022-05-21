using Domain.Models;
using Domain.Models.Meetings;
using Domain.Models.Sanctions;
using Domain.Networking;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using System;
using System.Text;

namespace Domain.Automation
{
    internal sealed class MeetingsAutomation : ITaskAutomation<MeetingsEntity>
    {

        private MeetingsEntity _oldObj;
        private MeetingsEntity _newObj;

        private readonly BaseRepository _repository;
        public MeetingsAutomation(BaseRepository repository)
        {
            _repository = repository;
        }


        public void Invoke(AutomationAction action)
        {
            switch (action)
            {
                case AutomationAction.OnUpdate:
                    OnUpdate();
                    return;
                default:
                    return;
            }
        }

        public ITaskAutomation<MeetingsEntity> SetData(MeetingsEntity oldObj, MeetingsEntity newObj)
        {
            _oldObj = oldObj;
            _newObj = newObj;
            return this;
        }

        private void OnUpdate()
        {
            if (_newObj == null || _oldObj == null) return;
            if (!string.IsNullOrEmpty(_newObj.SecondMeetingOutcome) && !_newObj.SecondMeetingOutcome.Equals(_oldObj.SecondMeetingOutcome))
            {
                _repository.Execute($"{GetSanctionQuery()}{GetTerminationQuery()}");
            }
        }

        private string GetSanctionQuery()
        {
            if (_newObj.SecondMeetingOutcome.Equals("NFA") || _newObj.SecondMeetingOutcome.Equals("Cancelled")) return string.Empty;
            var stringBuilder = new StringBuilder();
            var sanction = new SanctionEntry().Init().SetEmployee(_newObj).SetSanction(_newObj.SecondMeetingOutcome, _newObj.SecondMeetingDate);
            var timelineEntry = new Timeline().Create(sanction.EmployeeID, TimelineOrigin.Sanctions);
            timelineEntry.EventMessage = $"{sanction.Sanction} has been recorded by {Environment.UserName} and is active until {sanction.SanctionEndDate.ToString(DataStorage.ShortPreviewDateFormat)}";

            stringBuilder.Append($"INSERT INTO timeline {timelineEntry.GetHeader()} VALUES {timelineEntry.GetValues()};");
            stringBuilder.Append($@"UPDATE sanctions SET sanctionEndDate = '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}', overriden = '1', overridenBy = '{Environment.UserName}', overridenAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' 
                                                where employeeID = '{_newObj.EmployeeID}' AND sanctionEndDate > '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}' AND meetingType = '{(int)sanction.MeetingType}';");

            stringBuilder.Append($@"INSERT INTO sanctions {sanction.GetHeader()} VALUES {sanction.GetValues()};");
            return stringBuilder.ToString();
        }

        private string GetTerminationQuery()
        {
            if (!_newObj.SecondMeetingOutcome.Equals("Termination")) return string.Empty;

            var count = _repository.GetCachedScalar<int>($"SELECT COUNT(*) FROM awal WHERE awalStatus in ({(int)AwalStatus.Active}, {(int)AwalStatus.Pending}) AND employeeID = '{_newObj.EmployeeID}';");

            if (count > 0)
            {
                WebHook.PostAsync(DataStorage.AppSettings.AwalChanelWebHook, $"Hello, please close AWAL case for {_newObj.EmployeeID} if exists");
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"UPDATE meetings SET meetingStatus = 'Cancelled', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE meetingStatus in ('Open', 'Pending') AND employeeID = '{_newObj.EmployeeID}' AND id != '{_newObj.ID}';");
            stringBuilder.Append($"UPDATE custom_meetings SET meetingStatus = 'Cancelled', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE meetingStatus in ('Open', 'Pending') AND (claimantID = '{_newObj.EmployeeID}' OR respondentID = '{_newObj.EmployeeID}');");
            stringBuilder.Append($"UPDATE awal SET awalStatus = '{(int)AwalStatus.Cancelled}', outcome = 'Cancelled', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE employeeID = '{_newObj.EmployeeID}' AND awalStatus in ({(int)AwalStatus.Pending}, {(int)AwalStatus.Active});");

            return stringBuilder.ToString();
        }
    }
}
