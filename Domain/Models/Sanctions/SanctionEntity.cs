using Domain.DataManager;
using Domain.Extensions;
using Domain.Factory;
using Domain.Interfaces;
using Domain.Models.CustomMeetings;
using Domain.Storage;
using Domain.Types;
using System;

namespace Domain.Models.Sanctions
{
    public struct SanctionEntity : IDataImportObject
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string Shift { get; set; }
        public string Manager { get; set; }
        public string Sanction { get; set; }
        public DateTime SanctionStartDate { get; set; }
        public DateTime SanctionEndDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public MeetingType MeetingType { get; set; }
        public bool Overriden { get; set; }
        public string OverridenBy { get; set; }
        public DateTime OverridenAt { get; set; }

        public string GetHeader() =>
             "(id, employeeID, userID, employeeName, shift, manager, sanction, sanctionStartDate, sanctionEndDate, createdBy, createdAt, meetingType, overriden, overridenBy, overridenAt)";
        
        public string GetValues()
        {
            return $@"('{ID}','{EmployeeID}','{UserID}','{EmployeeName.DbSanityCheck()}','{Shift}','{Manager.DbSanityCheck()}','{Sanction}','{SanctionStartDate.DbSanityCheck(DataStorage.ShortDBDateFormat)}',
                            '{SanctionEndDate.DbSanityCheck(DataStorage.ShortDBDateFormat)}','{CreatedBy}','{CreatedAt.DbSanityCheck(DataStorage.LongDBDateFormat)}','{(int)MeetingType}','{Convert.ToInt16(Overriden)}',
                            '{OverridenBy.DbSanityCheck()}', {OverridenAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)})";
        }

        public SanctionEntity Init()
        {
            ID = Guid.NewGuid().ToString();
            CreatedBy = Environment.UserName;
            CreatedAt = DateTime.Now;
            Overriden = false;
            return this;
        }

        public object ReadFromCSV(string[] fields, DataMap dataMap)
        {
            ID = dataMap.GetStrValue(nameof(ID), fields);
            if (string.IsNullOrEmpty(ID)) ID = Guid.NewGuid().ToString();

            EmployeeID = dataMap.GetStrValue(nameof(EmployeeID), fields);
            UserID = dataMap.GetStrValue(nameof(UserID), fields);
            EmployeeName = dataMap.GetStrValue(nameof(EmployeeName), fields);
            Shift = dataMap.GetStrValue(nameof(Shift), fields);
            Manager = dataMap.GetStrValue(nameof(Manager), fields);
            Enum.TryParse(dataMap.GetStrValue(nameof(MeetingType), fields), out MeetingType mType);
            MeetingType = mType;

            Sanction = dataMap.GetStrValue(nameof(Sanction), fields);
            SanctionStartDate = dataMap.GetDateValue(nameof(SanctionStartDate), fields);
            SanctionEndDate = dataMap.GetDateValue(nameof(SanctionEndDate), fields);
            CreatedAt = dataMap.GetDateValue(nameof(CreatedAt), fields);
            CreatedBy = dataMap.GetStrValue(nameof(CreatedBy), fields);
            OverridenAt = dataMap.GetDateValue(nameof(OverridenAt), fields);
            OverridenBy = dataMap.GetStrValue(nameof(OverridenBy), fields);

            Overriden = !string.IsNullOrEmpty(OverridenBy);
            return this;
        }

        public SanctionEntity SetEmployee(IEmployee empl)
        {
            EmployeeID = empl.EmployeeID;
            UserID = empl.UserID;
            EmployeeName = empl.EmployeeName;
            Shift = empl.ShiftPattern;
            Manager = empl.ManagerName;
            return this;
        }

        public SanctionEntity SetEmployee(CustomMeetingEntity empl)
        {
            EmployeeID = empl.RespondentID;
            UserID = empl.RespondentUserID;
            EmployeeName = empl.RespondentName;
            Shift = empl.RespondentShift;
            Manager = empl.RespondentManager;
            return this;
        }

        public SanctionEntity SetSanction(string sanction, DateTime startDate)
        {
            Sanction = sanction;
            SanctionStartDate = startDate;
            MeetingType = SanctionManager.GetMeetingType(sanction);
            SanctionEndDate = SanctionManager.GetSanctionEndDate(this);
            return this;
        }
    }
}
