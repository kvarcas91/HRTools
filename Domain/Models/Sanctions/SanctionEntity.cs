using Domain.DataManager;
using Domain.Extensions;
using Domain.Models.DataSnips;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;

namespace Domain.Models.Sanctions
{
    public struct SanctionEntity
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

        public SanctionEntity Init()
        {
            ID = Guid.NewGuid().ToString();
            CreatedBy = Environment.UserName;
            CreatedAt = DateTime.Now;
            return this;
        }

        public SanctionEntity SetEmployee(Roster empl)
        {
            EmployeeID = empl.EmployeeID;
            UserID = empl.UserID;
            EmployeeName = empl.EmployeeName;
            Shift = empl.ShiftPattern;
            Manager = empl.ManagerName;
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

        internal string GetDbInsertHeader()
        {
            return @"(id, employeeID, userID, employeeName, shift, manager, sanction, sanctionStartDate, sanctionEndDate, createdBy, createdAt, 
                            meetingType, overriden)";
        }

        internal string GetDbInsertValues()
        {
            return $@"('{ID}','{EmployeeID}','{UserID}','{EmployeeName.DbSanityCheck()}','{Shift}','{Manager.DbSanityCheck()}','{Sanction}','{SanctionStartDate.DbSanityCheck(DataStorage.ShortDBDateFormat)}',
                            '{SanctionEndDate.DbSanityCheck(DataStorage.ShortDBDateFormat)}','{CreatedBy}','{CreatedAt.DbSanityCheck(DataStorage.LongDBDateFormat)}','{(int)MeetingType}','0')";
        }
    }
}
