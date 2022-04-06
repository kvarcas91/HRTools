using Domain.Models.DataSnips;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;

namespace Domain.Models.AWAL
{
    public class AwalEntity
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentID { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public string EmploymentType { get; set; }
        public string ManagerName { get; set; }
        public string ShiftPattern { get; set; }
        public AwalStatus AwalStatus { get; set; }
        public DateTime FirstNCNSDate { get; set; }
        public DateTime Awal1SentDate { get; set; }
        public DateTime Awal2SentDate { get; set; }
        public DateTime DisciplinaryDate { get; set; }
        public string Outcome { get; set; }
        public DateTime UKPendingEndDate { get; set; }
        public string CloseBridge { get; set; }
        public string BridgeCreatedBy { get; set; }
        public DateTime BridgeCreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public AwalEntity() { }

        public AwalEntity(Roster empl)
        {
            ID = Guid.NewGuid().ToString();
            EmployeeID = empl.EmployeeID;
            UserID = empl.UserID;
            EmployeeName = empl.EmployeeName;
            DepartmentID = empl.DepartmentID;
            EmploymentStartDate = empl.EmploymentStartDate;
            EmploymentType = empl.EmploymentType;
            ManagerName = empl.ManagerName;
            ShiftPattern = empl.ShiftPattern;
            AwalStatus = AwalStatus.Active;
        }

        public AwalEntity Add(AwalEntry entry)
        {
            FirstNCNSDate = entry.FirstNCNSDate;
            Awal1SentDate = entry.Awal1SentDate;
            Awal2SentDate = entry.Awal2SentDate;
            return this;
        }

    }
}
