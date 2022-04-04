using Domain.Interfaces;
using Domain.IO;
using Domain.Storage;
using System;

namespace Domain.Models
{
    public class Roster : ISearchable, IWritable
    {

        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentID { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public string LastHireDate
        {
            get
            {
                return EmploymentStartDate.Equals(DateTime.MinValue) ? "unknown" : EmploymentStartDate.ToString(DataStorage.ShortPreviewDateFormat);
            }
            set { }
            
        }
        public string EmploymentType { get; set; }
        public string ManagerName { get; set; }
        public string AgencyCode { get; set; }
        public string JobTitle { get; set; }
        public string FCLM { get; set; }
        public string ShiftPattern { get; set; }
        public int TenureInDays { get; set; }

        public Roster()
        {
            EmployeeID = string.Empty;
            UserID = string.Empty;
            EmployeeName = string.Empty;
            DepartmentID = string.Empty;
            ManagerName = string.Empty;
            EmploymentType = string.Empty;
            AgencyCode = string.Empty;
            JobTitle = string.Empty;
            FCLM = string.Empty;
            ShiftPattern = string.Empty;
            TenureInDays = -1;
        }

        public void SetTenure()
        {
            if (EmploymentStartDate.Equals(DateTime.MinValue)) return;

            TenureInDays = (DateTime.Now - EmploymentStartDate).Days;
        }

        public bool HasValue(string key)
        {
            return !string.IsNullOrEmpty(key) && (EmployeeID.ToUpper().Contains(key.ToUpper()) || UserID.ToUpper().Contains(key.ToUpper()) || EmployeeName.ToUpper().Contains(key.ToUpper()) ||
                ManagerName.ToUpper().Contains(key.ToUpper())|| JobTitle.ToUpper().Contains(key.ToUpper()) || ShiftPattern.ToUpper().Contains(key.ToUpper()));
        }

        public string GetHeader()
        {
            return "EmployeeID,UserID,EmployeeName,DepartmentID,EmploymentStartDate,EmploymentType,ManagerName,AgencyCode,JobTitle,FCLM,ShiftPattern";
        }

        public string GetRow()
        {

            return $"{FileHelper.VerifyCSV(EmployeeID)},{FileHelper.VerifyCSV(UserID)},{FileHelper.VerifyCSV(EmployeeName)},{FileHelper.VerifyCSV(DepartmentID)},{FileHelper.VerifyCSV(EmploymentStartDate.ToString(DataStorage.ShortPreviewDateFormat))},{FileHelper.VerifyCSV(EmploymentType)},{FileHelper.VerifyCSV(ManagerName)},{FileHelper.VerifyCSV(AgencyCode)},{FileHelper.VerifyCSV(JobTitle)},{FileHelper.VerifyCSV(FCLM)}, {FileHelper.VerifyCSV(ShiftPattern)}";
        }
    }
}
