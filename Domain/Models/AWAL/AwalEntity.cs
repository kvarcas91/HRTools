using Domain.Extensions;
using Domain.Factory;
using Domain.Storage;
using Domain.Types;
using System;

namespace Domain.Models.AWAL
{
    public class AwalEntity : IDataImportObject
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentID { get; set; }
        public string EmploymentType { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public string ManagerName { get; set; }
        public string ShiftPattern { get; set; }
        public AwalStatus AwalStatus { get; set; }
        public DateTime FirstNCNSDate { get; set; }
        public DateTime Awal1SentDate { get; set; }
        public DateTime Awal2SentDate { get; set; }
        public DateTime DisciplinaryDate { get; set; }
        public string Outcome { get; set; }
        public string ReasonForClosure { get; set; }
        public string BridgeCreatedBy { get; set; }
        public DateTime BridgeCreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public AwalEntity() 
        {
            CreatedAt = DateTime.Now;
            CreatedBy = Environment.UserName;
        }

        public AwalEntity(Roster empl) : this()
        {
            ID = Guid.NewGuid().ToString();
            EmployeeID = empl.EmployeeID;
            UserID = empl.UserID;
            EmployeeName = empl.EmployeeName;
            DepartmentID = empl.DepartmentID;
            EmploymentStartDate = empl.EmploymentStartDate;
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

        public string GetHeader()
        {
            return "(id, employeeID, userID, employeeName, departmentID, employmentType, employmentStartDate,managerName, shiftPattern,awalStatus,firstNCNSDate,awal1SentDate,awal2SentDate,disciplinaryDate,outcome,reasonForClosure,bridgeCreatedBy,bridgeCreatedAt,createdBy,createdAt,updatedAt,updatedBy)";
        }
        public string GetValues()
        {
            return $@"('{ID}','{EmployeeID}','{UserID}','{EmployeeName.DbSanityCheck()}','{DepartmentID}','{EmploymentType}',{EmploymentStartDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},
                        '{ManagerName.DbSanityCheck()}','{ShiftPattern}','{(int)AwalStatus}',{FirstNCNSDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},
                        {Awal1SentDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},{Awal2SentDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},
                        {DisciplinaryDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},'{Outcome}','{ReasonForClosure.DbSanityCheck()}', '{BridgeCreatedBy}',
                        {BridgeCreatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},'{CreatedBy}',{CreatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},
                        {UpdatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)}, '{UpdatedBy}')";
        }

        public object ReadFromCSV(string[] fields, DataMap dataMap)
        {
            ID = dataMap.GetStrValue(nameof(ID), fields);
            if (string.IsNullOrEmpty(ID)) ID = Guid.NewGuid().ToString();
            EmployeeID = dataMap.GetStrValue(nameof(EmployeeID), fields);
            UserID = dataMap.GetStrValue(nameof(UserID), fields);
            EmployeeName = dataMap.GetStrValue(nameof(EmployeeName), fields);
            DepartmentID = dataMap.GetStrValue(nameof(DepartmentID), fields);
            EmploymentType = dataMap.GetStrValue(nameof(EmploymentType), fields);
            EmploymentStartDate = dataMap.GetDateValue(nameof(EmploymentStartDate), fields);
            ShiftPattern = dataMap.GetStrValue(nameof(ShiftPattern), fields);
            ManagerName = dataMap.GetStrValue(nameof(ManagerName), fields);
            Enum.TryParse(dataMap.GetStrValue(nameof(AwalStatus), fields), out AwalStatus status);
            AwalStatus = status;
            FirstNCNSDate = dataMap.GetDateValue(nameof(FirstNCNSDate), fields);
            Awal1SentDate = dataMap.GetDateValue(nameof(Awal1SentDate), fields);
            Awal2SentDate = dataMap.GetDateValue(nameof(Awal2SentDate), fields);
            DisciplinaryDate = dataMap.GetDateValue(nameof(DisciplinaryDate), fields);
            Outcome = dataMap.GetStrValue(nameof(Outcome), fields);
            ReasonForClosure = dataMap.GetStrValue(nameof(ReasonForClosure), fields);
            if (!string.IsNullOrEmpty(ReasonForClosure))
            {

            }
            BridgeCreatedAt = dataMap.GetDateValue(nameof(BridgeCreatedAt), fields);
            BridgeCreatedBy = dataMap.GetStrValue(nameof(BridgeCreatedBy), fields);
            CreatedAt = dataMap.GetDateValue(nameof(CreatedAt), fields);
            CreatedBy = dataMap.GetStrValue(nameof(CreatedBy), fields);
            UpdatedAt = dataMap.GetDateValue(nameof(UpdatedAt), fields);
            UpdatedBy = dataMap.GetStrValue(nameof(UpdatedBy), fields);

            return this;
        }

    }
}
