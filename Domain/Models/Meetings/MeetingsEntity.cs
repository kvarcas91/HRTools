using Domain.Extensions;
using Domain.Factory;
using Domain.Interfaces;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;

namespace Domain.Models.Meetings
{
    public class MeetingsEntity : IEmployee, IDataImportObject, IWritable, ISearchable
    {
        public string ID { get; set; }
        public int CaseAge { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string ShiftPattern { get; set; }
        public string ManagerName { get; set; }
        public string DepartmentID { get; set; }
        public MeetingType MeetingType { get; set; }
        public DateTime FirstMeetingDate { get; set; }
        public string FirstMeetingOutcome { get; set; }
        public DateTime SecondMeetingDate { get; set; }
        public string SecondMeetingOutcome { get; set; }
        public DateTime RTWDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string MeetingStatus { get; set; }
        public bool IsERCaseStatusOpen { get; set; }
        public bool Paperless { get; set; }
        public string MeetingProgress { get; set; }
        public List<string> FirstMeetingOutcomeList { get; private set; }
        public List<string> SecondMeetingOutcomeList { get; private set; }

        public MeetingsEntity()
        {
            IsERCaseStatusOpen = false;
            ID = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            CreatedBy = Environment.UserName;
            MeetingStatus = "Open";
            FirstMeetingOutcome = string.Empty;
            SecondMeetingOutcome = string.Empty;
        }

        public MeetingsEntity(ERMeetingImportMap entry, Roster empl) : this()
        {
            if (empl != null)
            {
                UserID = empl.UserID;
                EmployeeName = empl.EmployeeName;
                ShiftPattern = empl.ShiftPattern;
                ManagerName = empl.ManagerName;
                DepartmentID = empl.DepartmentID;
            }

            ID = entry.CaseNumber;

            EmployeeID = entry.EmployeeID;
            CreatedBy = "er_import";
            MeetingType = entry.MeetingType;
            Paperless = false;
            SetProgress(entry.Task);
            if (string.IsNullOrEmpty(MeetingProgress))
            {
                FirstMeetingDate = entry.DueDate;
                return;
            }
            
            if (MeetingProgress.Contains("Informal") || MeetingProgress.Contains("Investigation")) FirstMeetingDate = entry.DueDate;
            else SecondMeetingDate = entry.DueDate;
            
        }

        public MeetingsEntity(MeetingsEntry entry, Roster empl) : this()
        {
            EmployeeID = empl.EmployeeID;
            UserID = empl.UserID;
            EmployeeName = empl.EmployeeName;
            ShiftPattern = empl.ShiftPattern;
            ManagerName = empl.ManagerName;
            DepartmentID = empl.DepartmentID;
            Enum.TryParse(entry.MeetingType, out MeetingType mType);
            MeetingType = mType;
            ID = entry.ID.Trim();
            FirstMeetingDate = entry.FirstMeetingDate;
            SetProgress();
        }

        public object ReadFromCSV(string[] fields, DataMap dataMap)
        {
            ID = dataMap.GetStrValue(nameof(ID), fields);
            if (string.IsNullOrEmpty(ID)) ID = Guid.NewGuid().ToString();
           
            EmployeeID = dataMap.GetStrValue(nameof(EmployeeID), fields);
            UserID = dataMap.GetStrValue(nameof(UserID), fields);
            EmployeeName = dataMap.GetStrValue(nameof(EmployeeName), fields);
            ShiftPattern = dataMap.GetStrValue(nameof(ShiftPattern), fields);
            DepartmentID = dataMap.GetStrValue(nameof(DepartmentID), fields);
            ManagerName = dataMap.GetStrValue(nameof(ManagerName), fields);
            Enum.TryParse(dataMap.GetStrValue(nameof(MeetingType), fields), out MeetingType mType);
            MeetingType = mType;

            FirstMeetingDate = dataMap.GetDateValue(nameof(FirstMeetingDate), fields);
            FirstMeetingOutcome = dataMap.GetStrValue(nameof(FirstMeetingOutcome), fields);
            SecondMeetingDate = dataMap.GetDateValue(nameof(SecondMeetingDate), fields);
            SecondMeetingOutcome = dataMap.GetStrValue(nameof(SecondMeetingOutcome), fields);
            MeetingStatus = dataMap.GetStrValue(nameof(MeetingStatus), fields);
            CreatedAt = dataMap.GetDateValue(nameof(CreatedAt), fields);
            CreatedBy = dataMap.GetStrValue(nameof(CreatedBy), fields);
            UpdatedAt = dataMap.GetDateValue(nameof(UpdatedAt), fields);
            UpdatedBy = dataMap.GetStrValue(nameof(UpdatedBy), fields);
            IsERCaseStatusOpen = dataMap.GetBoolValue(nameof(IsERCaseStatusOpen), fields);
            Paperless = dataMap.GetBoolValue(nameof(Paperless), fields);

            return this;
        }

        public string GetHeader() => 
            "(id,employeeID,userID,employeeName,shiftPattern,managerName, departmentID,meetingType,firstMeetingDate,firstMeetingOutcome,secondMeetingDate,secondMeetingOutcome,rtwDate,meetingStatus,createdAt,createdBy,updatedAt,updatedBy,isERCaseStatusOpen,paperless)";

        public string GetValues() =>
            $@"('{ID}','{EmployeeID}','{UserID}','{EmployeeName.DbSanityCheck()}','{ShiftPattern}','{ManagerName.DbSanityCheck()}', '{DepartmentID}', '{(int)MeetingType}',{FirstMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},'{FirstMeetingOutcome}',
                {SecondMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},'{SecondMeetingOutcome}',{RTWDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},
                '{MeetingStatus}',{CreatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},'{CreatedBy}',
                {UpdatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},'{UpdatedBy}','{Convert.ToUInt16(IsERCaseStatusOpen)}','{Convert.ToUInt16(Paperless)}')";

        private void SetProgress(string task)
        {
            switch (MeetingType)
            {
                case MeetingType.Health:
                    MeetingProgress = task.Contains("Informal") ? "Informal Health Review" : "Formal Health Review";
                    FirstMeetingOutcomeList = new List<string> { "NFA", "Proceed to FHRM" };
                    SecondMeetingOutcomeList = new List<string> { "NFA", "1st LoC", "2nd LoC", "3rd LoC", "Termination" };
                    break;
                case MeetingType.Disciplinary:
                    MeetingProgress = task.Contains("Investigation") ? "Investigation" : "Disciplinary";
                    FirstMeetingOutcomeList = new List<string> { "NFA", "Proceed to Disciplinary" };
                    SecondMeetingOutcomeList = new List<string> { "NFA", "Verbal Warning", "Written Warning", "Final Warning", "Termination" };
                    break;
            }
        }

        public void SetProgress()
        {
            switch(MeetingType)
            {
                case MeetingType.Health:
                    MeetingProgress = (string.IsNullOrEmpty(FirstMeetingOutcome) && SecondMeetingDate == DateTime.MinValue) || FirstMeetingOutcome.Equals("NFA") || FirstMeetingOutcome.Equals("Cancelled") ? "Informal Health Review" : "Formal Health Review";
                    FirstMeetingOutcomeList = new List<string> { "NFA", "Proceed to FHRM" };
                    SecondMeetingOutcomeList = new List<string> { "NFA", "1st LoC", "2nd LoC", "3rd LoC", "Termination" };
                    break;
                case MeetingType.Disciplinary:
                    MeetingProgress = (string.IsNullOrEmpty(FirstMeetingOutcome) && SecondMeetingDate == DateTime.MinValue) || FirstMeetingOutcome.Equals("NFA") || FirstMeetingOutcome.Equals("Cancelled") ? "Investigation" : "Disciplinary";
                    FirstMeetingOutcomeList = new List<string> { "NFA", "Proceed to Disciplinary" };
                    SecondMeetingOutcomeList = new List<string> { "NFA", "Verbal Warning", "Written Warning", "Final Warning", "Termination" };
                    break;
            }
        }

        public string GetDataHeader() =>
            "Case Number, Case Age, EmployeeID, UserID, Name, Manager, Shift, Department, Meeting Type, FirstMeetingDate, FirstMeetingOutcome, SecondMeetingDate, SecondMeetingOutcome, RTWDate, MeetingStatus, Paperless";

        public string GetDataRow()
        {
            return $"{ID.VerifyCSV()},{CaseAge},{EmployeeID.VerifyCSV()},{UserID.VerifyCSV()},{EmployeeName.VerifyCSV()},{ManagerName.VerifyCSV()},{ShiftPattern.VerifyCSV()},{DepartmentID.VerifyCSV()}," +
                $"{MeetingProgress.VerifyCSV()},{FirstMeetingDate.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()},{FirstMeetingOutcome.VerifyCSV()}," +
                $"{SecondMeetingDate.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()},{SecondMeetingOutcome.VerifyCSV()},{RTWDate.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()},{MeetingStatus.VerifyCSV()},{Paperless}";
        }

        public void SetAge()
        {
            DateTime startDate = FirstMeetingDate != DateTime.MinValue ? FirstMeetingDate : CreatedAt;
            if (ID.Equals("35082469"))
            {

            }
            DateTime endDate = GetEndAge(startDate);
           
            CaseAge = (startDate == DateTime.MinValue || endDate == DateTime.MinValue) ? 0 : (endDate - startDate).Days;
        }

        private DateTime GetEndAge(DateTime startDate)
        {
            if (!string.IsNullOrEmpty(FirstMeetingOutcome) && string.IsNullOrEmpty(SecondMeetingOutcome))
            {
                switch (FirstMeetingOutcome)
                {
                    case "NFA":
                    case "Cancelled":
                        return UpdatedAt != DateTime.MinValue ? UpdatedAt : FirstMeetingDate;
                    default:
                        return !MeetingStatus.Equals("Closed") ? DateTime.Today : FirstMeetingDate != DateTime.MinValue ? FirstMeetingDate : startDate;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(SecondMeetingOutcome))
                {
                    return !MeetingStatus.Equals("Closed") ? DateTime.Today : UpdatedAt != DateTime.MinValue ? UpdatedAt : SecondMeetingDate != DateTime.MinValue ? SecondMeetingDate : startDate;
                }
            }

            if (!string.IsNullOrEmpty(SecondMeetingOutcome))
            {
                switch (SecondMeetingOutcome)
                {
                    case "NFA":
                    case "Cancelled":
                        return UpdatedAt != DateTime.MinValue ? UpdatedAt : SecondMeetingDate;
                    default:
                        return !MeetingStatus.Equals("Closed") ? DateTime.Today : SecondMeetingDate != DateTime.MinValue ? SecondMeetingDate : startDate;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(SecondMeetingOutcome))
                {
                    return !MeetingStatus.Equals("Closed") ? DateTime.Today : UpdatedAt != DateTime.MinValue ? UpdatedAt : SecondMeetingDate != DateTime.MinValue ? SecondMeetingDate : startDate;
                }
            }

            return DateTime.MinValue;

        }

        public bool HasValue(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            bool hasValue = true;


            var arr = key.Trim().Split(new char[] { ' ' });

            foreach (var item in arr)
            {
                hasValue &= (EmployeeID != null && EmployeeID.Contains(item)) || (UserID != null && UserID.ToUpper().Contains(item.ToUpper())) || (EmployeeName != null && EmployeeName.ToUpper().Contains(item.ToUpper())) ||
                    (DepartmentID != null && DepartmentID.Contains(item)) || (ManagerName != null && ManagerName.ToUpper().Contains(item.ToUpper())) || (ID != null && ID.ToUpper().Contains(item.ToUpper())) ||
                    (ShiftPattern != null && ShiftPattern.ToUpper().Contains(item.ToUpper()));
            }

            return hasValue;
        }
    }
}
