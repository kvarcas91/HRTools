using Domain.Factory;
using Domain.Interfaces;
using Domain.IO;
using Domain.Models.AWAL;
using Domain.Models.Resignations;
using Domain.Storage;
using System;

namespace Domain.Models
{
    public class Roster : ISearchable, IWritable, IDataImportObject, IEmployee
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
        }

        public Roster (AwalEntity awal)
        {
            EmployeeID = awal.EmployeeID;
            UserID = awal.UserID;
            EmployeeName = awal.EmployeeName;
            DepartmentID = awal.DepartmentID;
            ManagerName = awal.ManagerName;
            EmploymentType = awal.EmploymentType;
            AgencyCode = string.Empty;
            JobTitle = string.Empty;
            EmploymentStartDate = awal.EmploymentStartDate;
            ShiftPattern = awal.ShiftPattern;
            FCLM = string.Empty;
        }

        public Roster (ResignationEntity entity)
        {
            EmployeeID = entity.EmployeeID;
            UserID = entity.UserID;
            EmployeeName = entity.Name;
            DepartmentID = string.Empty;
            ManagerName = entity.Manager;
            EmploymentType = string.Empty;
            AgencyCode = string.Empty;
            EmploymentStartDate= entity.EmploymentStartDate;
            ShiftPattern = entity.Shift;
            FCLM = string.Empty;
        }

        public bool HasValue(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            bool hasValue = true;

            
            var arr = key.Trim().Split(new char[] {' '});

            foreach (var item in arr)
            {
                hasValue &= (EmployeeID != null && EmployeeID.Contains(item)) || (UserID != null && UserID.ToUpper().Contains(item.ToUpper())) || (EmployeeName != null && EmployeeName.ToUpper().Contains(item.ToUpper())) ||
                    (DepartmentID != null && DepartmentID.Contains(item)) || (ManagerName != null && ManagerName.ToUpper().Contains(item.ToUpper())) || (JobTitle != null && JobTitle.ToUpper().Contains(item.ToUpper())) ||
                    (ShiftPattern != null && ShiftPattern.ToUpper().Contains(item.ToUpper()));
            }

            return hasValue;
        }

        public string GetDataHeader()
        {
            return "EmployeeID,UserID,EmployeeName,DepartmentID,EmploymentStartDate,EmploymentType,ManagerName,AgencyCode,JobTitle,FCLM,ShiftPattern";
        }

        public string GetDataRow()
        {
            return $"{FileHelper.VerifyCSV(EmployeeID)},{FileHelper.VerifyCSV(UserID)},{FileHelper.VerifyCSV(EmployeeName)},{FileHelper.VerifyCSV(DepartmentID)}," +
                $"{FileHelper.VerifyCSV(EmploymentStartDate.ToString(DataStorage.ShortPreviewDateFormat))},{FileHelper.VerifyCSV(EmploymentType)},{FileHelper.VerifyCSV(ManagerName)},{FileHelper.VerifyCSV(AgencyCode)}," +
                $"{FileHelper.VerifyCSV(JobTitle)},{FileHelper.VerifyCSV(FCLM)}, {FileHelper.VerifyCSV(ShiftPattern)}";
        }

        public object ReadFromCSV(string[] fields, DataMap dataMap)
        {
            EmployeeID = dataMap.GetStrValue(nameof(EmployeeID), fields);
            UserID = dataMap.GetStrValue(nameof(UserID), fields);
            EmployeeName = dataMap.GetStrValue(nameof(EmployeeName), fields);
            DepartmentID = dataMap.GetStrValue(nameof(DepartmentID), fields);
            LastHireDate = dataMap.GetStrValue(nameof(LastHireDate), fields);
            EmploymentType = dataMap.GetStrValue(nameof(EmploymentType), fields);
            ManagerName = dataMap.GetStrValue(nameof(ManagerName), fields);
            AgencyCode = dataMap.GetStrValue(nameof(AgencyCode), fields);
            JobTitle = dataMap.GetStrValue(nameof(JobTitle), fields);
            FCLM = dataMap.GetStrValue(nameof(FCLM), fields);
            ShiftPattern = dataMap.GetStrValue(nameof(ShiftPattern), fields);
            EmploymentStartDate = dataMap.GetDateValue(nameof(EmploymentStartDate), fields);

            return this;
        }

        public string GetValues()
        {
            return string.Empty;
        }

        public string GetHeader() => GetDataHeader();
    }
}
