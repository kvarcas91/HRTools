using Domain.Factory;
using System.Collections.Generic;

namespace Domain.Models
{
    public class RosterImportMap : IDataMap
    {

        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentID { get; set; }
        public string EmploymentStartDate { get; set; }
        public string EmploymentType { get; set; }
        public string ManagerName { get; set; }
        public string AgencyCode { get; set; }
        public string JobTitle { get; set; }
        public string FCLM { get; set; }
        public string ShiftPattern { get; set; }

        public RosterImportMap()
        {
            EmployeeID = "EmployeeID";
            EmployeeName = "EmployeeName";
            DepartmentID = "DepartmentID";
            EmploymentStartDate = "EmploymentStartDate";
            UserID = "UserID";
            EmploymentType = "EmploymentType";
            ManagerName = "ManagerName";
            AgencyCode = "AgencyCode";
            JobTitle = "JobTitle";
            FCLM = "FCLM";
            ShiftPattern = "ShiftPattern";
        }

        public Dictionary<string, string> GetRequiredHeaders()
        {
            return new Dictionary<string, string> 
            { 
                { "EmployeeID", EmployeeID }, { "UserID", UserID }, { "EmployeeName", EmployeeName }, { "DepartmentID", DepartmentID }, { "EmploymentStartDate", EmploymentStartDate },
                { "EmploymentType", EmploymentType }, { "ManagerName", ManagerName }, { "AgencyCode", AgencyCode }, { "JobTitle", JobTitle }, { "FCLM", FCLM }, {"ShiftPattern", ShiftPattern }
            };
        }
    }
}
