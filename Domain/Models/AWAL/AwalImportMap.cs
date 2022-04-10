using Domain.Factory;
using System.Collections.Generic;

namespace Domain.Models.AWAL
{
    public class AwalImportMap : IDataMap
    {
        public string EmployeeID { get; set; }
        public string ID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentID { get; set; }
        public string EmploymentStartDate { get; set; }
        public string EmploymentType { get; set; }
        public string ManagerName { get; set; }
        public string ShiftPattern { get; set; }
        public string AwalStatus { get; set; }
        public string FirstNCNSDate { get; set; }
        public string Awal1SentDate { get; set; }
        public string Awal2SentDate { get; set; }
        public string DisciplinaryDate { get; set; }
        public string Outcome { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedAt { get; set; }
        public string BridgeCreatedBy { get; set; }
        public string BridgeCreatedAt { get; set; }
        public string ReasonForClosure { get; set; }

        public Dictionary<string, string> GetRequiredHeaders()
        {
            return new Dictionary<string, string>
            {
                {"EmployeeID", EmployeeID }, {"ID", ID },{"UserID", UserID },{ "EmployeeName", EmployeeName}, {"DepartmentID", DepartmentID }, { "EmploymentStartDate", EmploymentStartDate}, {"EmploymentType", EmploymentType }, 
                { "ManagerName", ManagerName}, { "ShiftPattern", ShiftPattern}, {"AwalStatus", AwalStatus }, { "FirstNCNSDate", FirstNCNSDate}, { "Awal1SentDate", Awal1SentDate}, {"Awal2SentDate", Awal2SentDate }, 
                { "DisciplinaryDate", DisciplinaryDate}, {"Outcome", Outcome },{ "CreatedBy", CreatedBy},{ "CreatedAt", CreatedAt}, {"UpdatedBy", UpdatedBy },{ "UpdatedAt", UpdatedAt},{ "BridgeCreatedBy", BridgeCreatedBy},
                { "BridgeCreatedAt", BridgeCreatedAt},{ "ReasonForClosure", ReasonForClosure}
            };
        }

        public AwalImportMap SetDefaultValues()
        {
            ID = "id";
            EmployeeID = "employeeID";
            EmployeeName = "employeeName";
            UserID = "userID";
            DepartmentID = "departmentID";
            EmploymentStartDate = "employmentStartDate";
            EmploymentType = "employmentType";
            ManagerName = "managerName";
            ShiftPattern = "shiftPattern";
            AwalStatus = "awalStatus";
            FirstNCNSDate = "firstNCNSDate";
            Awal1SentDate = "awal1SentDate";
            Awal2SentDate = "awal2SentDate";
            DisciplinaryDate = "disciplinaryDate";
            Outcome = "outcome";
            CreatedBy = "createdBy";
            CreatedAt = "createdAt";
            UpdatedBy = "updatedBy";
            UpdatedAt = "updatedAt";
            BridgeCreatedAt = "bridgeCreatedAt";
            BridgeCreatedBy = "bridgeCreatedBy";
            ReasonForClosure = "closeBridge";

            return this;
        }
    }
}
