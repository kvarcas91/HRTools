using Domain.Models.Resignations;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using System;

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
            var query = $"{GetAwalReopenQuery()}";
            _repository.Execute(query);
        }

        public ITaskAutomation<ResignationEntity> SetData(ResignationEntity oldObj, ResignationEntity newObj)
        {
            _newObj = newObj;
            return this;
        }

        private string GetAwalReopenQuery() =>
             $"UPDATE awal SET awalStatus = '{(int)AwalStatus.Active}', outcome = NULL, updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE employeeID = '{_newObj.EmployeeID}' AND awalStatus = '{(int)AwalStatus.Resigned}';";

        private string GetAwalQuery() => 
            $"UPDATE awal SET awalStatus = '{(int)AwalStatus.Resigned}', outcome = 'Cancelled', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE employeeID = '{_newObj.EmployeeID}' AND awalStatus in ({(int)AwalStatus.Pending}, {(int)AwalStatus.Active});";

        private string GetMeetingsQuery() => 
            $"UPDATE meetings SET meetingStatus = 'Resigned', updatedBy = '(tool_automation)', updatedAt = '{DateTime.Now.ToString(DataStorage.LongDBDateFormat)}' WHERE employeeID = '{_newObj.EmployeeID}' AND meetingStatus in ('Open', 'Pending');";

        private string GetCustomMeetingsQuery() => "";

    }
}
