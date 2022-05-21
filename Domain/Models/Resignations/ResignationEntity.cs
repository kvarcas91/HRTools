using Domain.Extensions;
using Domain.Interfaces;
using Domain.Storage;
using System;

namespace Domain.Models.Resignations
{
    public class ResignationEntity : IWritable
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public string Shift { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public DateTime LastWorkingDay { get; set; }
        public string TTLink { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReasonForResignation { get; set; }

        public ResignationEntity()
        {
            ID = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            CreatedBy = Environment.UserName;
        }

        public ResignationEntity(ResignationEntry entry) : this()
        {
            LastWorkingDay = entry.LastWorkingDay;
            ReasonForResignation = entry.ReasonForResignation;
            TTLink = entry.TTLink;
        }

        public ResignationEntity AddEmployee(Roster empl)
        {
            EmployeeID = empl.EmployeeID;
            UserID = empl.UserID;
            Name = empl.EmployeeName;
            Manager = empl.ManagerName;
            Shift = empl.ShiftPattern;
            EmploymentStartDate = empl.EmploymentStartDate;
            return this;
        }

        public string GetHeader()
        {
            return $"(id,employeeID,userID,name,manager,shift,employmentStartDate,lastWorkingDay,ttLink,createdBy,createdAt,reasonForResignation)";
        }

        public string GetValues()
        {
            return $@"('{ID}','{EmployeeID}','{UserID}','{Name.DbSanityCheck()}','{Manager.DbSanityCheck()}','{Shift}','{EmploymentStartDate.DbSanityCheck(DataStorage.ShortDBDateFormat)}','{LastWorkingDay.DbSanityCheck(DataStorage.ShortDBDateFormat)}',
                        '{TTLink.DbSanityCheck()}','{CreatedBy}','{CreatedAt.DbSanityCheck(DataStorage.LongDBDateFormat)}','{ReasonForResignation}')";
        }

        public string GetDataHeader()
        {
            return "EmployeeID,UserID,Name,Manager,Shift,EmploymentStartDate,LastWorkingDay,ReasonForResignation,TTLink,CreatedBy,CreatedAt";
        }

        public string GetDataRow()
        {
            return $"{EmployeeID.VerifyCSV()},{UserID.VerifyCSV()},{Name.VerifyCSV()},{Manager.VerifyCSV()},{Shift.VerifyCSV()},{EmploymentStartDate.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()}," +
                $"{LastWorkingDay.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()},{ReasonForResignation.VerifyCSV()},{TTLink.VerifyCSV()},{CreatedBy.VerifyCSV()}," +
                $"{CreatedAt.ToString(DataStorage.LongPreviewDateFormat).VerifyCSV()}";
        }
    }
}
