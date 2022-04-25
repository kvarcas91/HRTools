using Domain.Models.Resignations;
using Domain.Networking;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using System;
using System.Text;

namespace Domain.Automation
{
    internal class ResignationsAutomation : ITaskAutomation<ResignationEntity>
    {
        private ResignationEntity _newObj;

        private readonly BaseRepository _repository;
        public ResignationsAutomation(BaseRepository repository)
        {
            _repository = repository;
        }

        public void Invoke(AutomationAction action)
        {
            switch (action)
            {
                case AutomationAction.OnIntake:
                    OnIntake();
                    break;
                case AutomationAction.OnUpdate:
                    OnUpdate();
                    break;
                default:
                    break;
            }
        }

        private void OnIntake()
        {
            if (_newObj == null) return;
            var query = $"{GetMeetingsQuery()}{GetAwalQuery()}{GetCustomMeetingsQuery()}";
            _repository.Execute(query);
        }

        private void OnUpdate()
        {
            if (_newObj == null) return;
            var query = $"{GetAwalReopenQuery()}{GetMeetingsReopenQuery()}";
            _repository.Execute(query);
        }

        public ITaskAutomation<ResignationEntity> SetData(ResignationEntity oldObj, ResignationEntity newObj)
        {
            _newObj = newObj;
            return this;
        }

        private string GetAwalReopenQuery() =>
             $"UPDATE awal SET awalStatus = '{(int)AwalStatus.Active}', outcome = NULL, updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE employeeID = '{_newObj.EmployeeID}' AND awalStatus = '{(int)AwalStatus.Resigned}';";

        private string GetMeetingsReopenQuery()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"UPDATE meetings SET meetingStatus = 'Open', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE employeeID = '{_newObj.EmployeeID}' AND meetingStatus = 'Resigned';");
            stringBuilder.Append($"UPDATE custom_meetings SET meetingStatus = 'Open', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE (claimantID = '{_newObj.EmployeeID}' OR respondentID = '{_newObj.EmployeeID}') AND meetingStatus = 'Resigned';");

            return stringBuilder.ToString();
        }
             
        private string GetAwalQuery()
        {
            var count = _repository.GetCachedScalar<int>($"SELECT COUNT(*) FROM awal WHERE awalStatus in ({(int)AwalStatus.Active}, {(int)AwalStatus.Pending}) AND employeeID = '{_newObj.EmployeeID}';");

            if (count > 0)
            {
                WebHook.PostAsync(DataStorage.AppSettings.AwalChanelWebHook, $"Hello, please close AWAL case for {_newObj.EmployeeID} if exists");
            }

            return $"UPDATE awal SET awalStatus = '{(int)AwalStatus.Resigned}', outcome = 'Cancelled', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE employeeID = '{_newObj.EmployeeID}' AND awalStatus in ({(int)AwalStatus.Pending}, {(int)AwalStatus.Active});";
        }

        private string GetMeetingsQuery() => 
            $"UPDATE meetings SET meetingStatus = 'Resigned', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE employeeID = '{_newObj.EmployeeID}' AND meetingStatus in ('Open', 'Pending');";

        private string GetCustomMeetingsQuery() =>
             $"UPDATE custom_meetings SET meetingStatus = 'Resigned', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE (claimantID = '{_newObj.EmployeeID}' OR respondentID = '{_newObj.EmployeeID}') AND meetingStatus in ('Open', 'Pending');";

    }
}
