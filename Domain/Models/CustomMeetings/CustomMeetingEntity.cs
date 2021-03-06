using Domain.Extensions;
using Domain.Factory;
using Domain.Interfaces;
using Domain.IO;
using Domain.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Models.CustomMeetings
{
    public class CustomMeetingEntity : IDataImportObject, IWritable
    {
        public string ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime ClosedAt { get; set; }
        public string ClosedBy { get; set; }

        public string MeetingStatus { get; set; }
        public string ExactCaseID { get; set; }
        public string MeetingType { get; set; }

        public string ClaimantID { get; set; }
        public string ClaimantName { get; set; }
        public string ClaimantUserID { get; set; }
        public string ClaimantManager { get; set; }
        public string ClaimantDepartment { get; set; }
        public string ClaimantShift { get; set; }
        public DateTime ClaimantEmploymentStartDate { get; set; }

        public string RespondentID { get; set; }
        public string RespondentName { get; set; }
        public string RespondentUserID { get; set; }
        public string RespondentManager { get; set; }
        public string RespondentDepartment { get; set; }
        public string RespondentShift { get; set; }
        public DateTime RespondentEmploymentStartDate { get; set; }

        public bool Paperless { get; set; }
        public bool IsUnionPresent { get; set; }
        public bool IsWIMRaised { get; set; }

        public DateTime FirstMeetingDate { get; set; }
        public string FirstMeetingOwner { get; set; }
        public string FirstMeetingHRSupport { get; set; }
        public string FirstMeetingOutcome { get; set; }

        public DateTime SecondMeetingDate { get; set; }
        public string SecondMeetingOwner { get; set; }
        public string SecondMeetingHRSupport { get; set; }
        public string SecondMeetingOutcome { get; set; }
        public string Actor { get; set; }

        public List<string> FirstMeetingOutcomeList { get; private set; }
        public List<string> SecondMeetingOutcomeList { get; private set; }

        public ObservableCollection<CaseFile> RelatedFiles { get; private set; }
        public int FileCount { get; set; }
        public DateTime CaseAge { get; set; }

        public CustomMeetingEntity()
        {
            RelatedFiles = new ObservableCollection<CaseFile>();
            ID = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            CreatedBy = Environment.UserName;
            MeetingStatus = "Open";
        }

        public CustomMeetingEntity(string meetingType, string exactCaseID) : this()
        {
            MeetingType = meetingType;
            ExactCaseID = exactCaseID;
        }

        public string GetHeader() =>
            $@"(id,createdAt,createdBy,updatedAt,updatedBy,closedAt,closedBy,meetingStatus,meetingType,exactCaseID,claimantID,claimantName,claimantUserID,claimantManager,claimantDepartment,
                claimantShift,claimantEmploymentStartDate,respondentID,respondentName,respondentUserID,respondentManager,respondentDepartment,respondentShift,respondentEmploymentStartDate,
                paperless,isUnionPresent,isWIMRaised,firstMeetingDate,firstMeetingOwner,firstMeetingHRSupport,firstMeetingOutcome,secondMeetingDate,secondMeetingOwner,secondMeetingHRSupport,
                secondMeetingOutcome)";

        public string GetValues() =>
            $@"('{ID}',{CreatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},'{CreatedBy.DbSanityCheck()}',{UpdatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},'{UpdatedBy.DbSanityCheck()}',
                {ClosedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)},'{ClosedBy.DbSanityCheck()}','{MeetingStatus}','{MeetingType}','{ExactCaseID.DbSanityCheck()}','{ClaimantID.DbSanityCheck()}',
                '{ClaimantName.DbSanityCheck()}','{ClaimantUserID.DbSanityCheck()}','{ClaimantManager.DbSanityCheck()}','{ClaimantDepartment.DbSanityCheck()}','{ClaimantShift.DbSanityCheck()}',
                {ClaimantEmploymentStartDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},'{RespondentID.DbSanityCheck()}','{RespondentName.DbSanityCheck()}','{RespondentUserID.DbSanityCheck()}',
                '{RespondentManager.DbSanityCheck()}','{RespondentDepartment.DbSanityCheck()}','{RespondentShift.DbSanityCheck()}',{RespondentEmploymentStartDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},
                '{Convert.ToInt16(Paperless)}','{Convert.ToInt16(IsUnionPresent)}','{Convert.ToInt16(IsWIMRaised)}',{FirstMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},
                '{FirstMeetingOwner.DbSanityCheck()}','{FirstMeetingHRSupport.DbSanityCheck()}','{FirstMeetingOutcome.DbSanityCheck()}',{SecondMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},
                '{SecondMeetingOwner.DbSanityCheck()}','{SecondMeetingHRSupport.DbSanityCheck()}','{SecondMeetingOutcome.DbSanityCheck()}')";

        public object ReadFromCSV(string[] fields, DataMap dataMap)
        {
            ID = dataMap.GetStrValue(nameof(ID), fields);
            if (string.IsNullOrEmpty(ID)) ID = Guid.NewGuid().ToString();

            CreatedAt = dataMap.GetDateValue(nameof(CreatedAt), fields);
            CreatedBy = dataMap.GetStrValue(nameof(CreatedBy), fields);
            UpdatedAt = dataMap.GetDateValue(nameof(UpdatedAt), fields);
            UpdatedBy = dataMap.GetStrValue(nameof(UpdatedBy), fields);
            ClosedAt = dataMap.GetDateValue(nameof(ClosedAt), fields);
            ClosedBy = dataMap.GetStrValue(nameof(ClosedBy), fields);

            MeetingStatus = dataMap.GetStrValue(nameof(MeetingStatus), fields);
            ExactCaseID = dataMap.GetStrValue(nameof(ExactCaseID), fields);
            MeetingType = dataMap.GetStrValue(nameof(MeetingType), fields);

            Paperless = dataMap.GetBoolValue(nameof(Paperless), fields);
            IsWIMRaised = dataMap.GetBoolValue(nameof(IsWIMRaised), fields);
            IsUnionPresent = dataMap.GetBoolValue(nameof(IsUnionPresent), fields);

            ClaimantID = dataMap.GetStrValue(nameof(ClaimantID), fields);
            ClaimantName = dataMap.GetStrValue(nameof(ClaimantName), fields);
            ClaimantDepartment = dataMap.GetStrValue(nameof(ClaimantDepartment), fields);
            ClaimantUserID = dataMap.GetStrValue(nameof(ClaimantUserID), fields);
            ClaimantShift = dataMap.GetStrValue(nameof(ClaimantShift), fields);
            ClaimantManager = dataMap.GetStrValue(nameof(ClaimantManager), fields);
            ClaimantEmploymentStartDate = dataMap.GetDateValue(nameof(ClaimantEmploymentStartDate), fields);

            RespondentID = dataMap.GetStrValue(nameof(RespondentID), fields);
            RespondentName = dataMap.GetStrValue(nameof(RespondentName), fields);
            RespondentDepartment = dataMap.GetStrValue(nameof(RespondentDepartment), fields);
            RespondentUserID = dataMap.GetStrValue(nameof(RespondentUserID), fields);
            RespondentShift = dataMap.GetStrValue(nameof(RespondentShift), fields);
            RespondentManager = dataMap.GetStrValue(nameof(RespondentManager), fields);
            RespondentEmploymentStartDate = dataMap.GetDateValue(nameof(RespondentEmploymentStartDate), fields);

            FirstMeetingDate = dataMap.GetDateValue(nameof(FirstMeetingDate), fields);
            FirstMeetingOwner = dataMap.GetStrValue(nameof(FirstMeetingOwner), fields);
            FirstMeetingHRSupport = dataMap.GetStrValue(nameof(FirstMeetingHRSupport), fields);
            FirstMeetingOutcome = dataMap.GetStrValue(nameof(FirstMeetingOutcome), fields);

            SecondMeetingDate = dataMap.GetDateValue(nameof(SecondMeetingDate), fields);
            SecondMeetingOwner = dataMap.GetStrValue(nameof(SecondMeetingOwner), fields);
            SecondMeetingHRSupport = dataMap.GetStrValue(nameof(SecondMeetingHRSupport), fields);
            SecondMeetingOutcome = dataMap.GetStrValue(nameof(SecondMeetingOutcome), fields);

            return this;
        }

        public CustomMeetingEntity SetClaimant(Roster claimant)
        {
            if (claimant == null) return this;

            ClaimantID = claimant.EmployeeID;
            ClaimantName = claimant.EmployeeName;
            ClaimantManager = claimant.ManagerName;
            ClaimantDepartment = claimant.DepartmentID;
            ClaimantShift = claimant.ShiftPattern;
            ClaimantUserID = claimant.UserID;
            ClaimantEmploymentStartDate = claimant.EmploymentStartDate;

            return this;
        }

        public CustomMeetingEntity SetRespondent(Roster respondent)
        {
            if (respondent == null) return this;

            RespondentID = respondent.EmployeeID;
            RespondentName = respondent.EmployeeName;
            RespondentManager = respondent.ManagerName;
            RespondentDepartment = respondent.DepartmentID;
            RespondentShift = respondent.ShiftPattern;
            RespondentUserID = respondent.UserID;
            RespondentEmploymentStartDate = respondent.EmploymentStartDate;

            return this;
        }

        public async void SetFiles()
        {
            var fileList = await FileHelper.GetFolderContentFromMeetingID(ID);
            RelatedFiles.Clear();
            foreach (var item in fileList)
            {
                RelatedFiles.Add(item);
            }
        }

        public void Prepare()
        {
            SetAge();
        }

        public void Prepare(string emplId)
        {
            SetFiles();
            SetAge();
            SetActor(emplId);
            SetOutcomes();
        }

        private void SetAge()
        {
            DateTime endDate = ClosedAt.Equals(DateTime.MinValue) ? DateTime.Now : ClosedAt;
            CaseAge = DateTime.Now.AddDays(-(endDate - CreatedAt).Days);
        }

        private void SetActor(string emplId)
        {
            if (ClaimantID == emplId) Actor = "Claimant";
            else Actor = "Respondent";
        }

        private void SetOutcomes()
        {
            switch (MeetingType)
            {
                case "Investigation":
                case "Adapt":
                case "Time Fraud":
                case "Eligibility":
                    FirstMeetingOutcomeList = new List<string> { "", "NFA", "Proceed to Disciplinary Hearing" };
                    SecondMeetingOutcomeList = new List<string> { "", "NFA", "Verbal Warning", "Written Warning", "Final Warning", "Termination" };
                    break;
                case "Appeal":
                    FirstMeetingOutcomeList = new List<string> { "", "Upheld", "Overturned" };
                    SecondMeetingOutcomeList = new List<string>();
                    break;
                case "Grievance":
                    FirstMeetingOutcomeList = new List<string> { "", "NFA", "Mediation", "Informal Resolution", "Proceed to Disciplinary Hearing" };
                    SecondMeetingOutcomeList = new List<string> { "", "NFA", "Verbal Warning", "Written Warning", "Final Warning", "Termination" };
                    break;
                case "Formal Probation Review":
                    FirstMeetingOutcomeList = new List<string> { "", "NFA", "Extension", "Probation Failed/Termination" };
                    SecondMeetingOutcomeList = new List<string>();
                    break;
                case "TWA":
                    FirstMeetingOutcomeList = new List<string> { "", "NFA", "Temporary Work Adjustments" };
                    SecondMeetingOutcomeList = new List<string>();
                    break;
                default:
                    FirstMeetingOutcomeList = new List<string>();
                    SecondMeetingOutcomeList = new List<string>();
                    break;
            }
        }

        public string GetDataHeader() =>
            "MeetingStatus,ExactID,MeetingType,ClaimantID,ClaimantName,ClaimantUserID,ClaimantManager,ClaimantDepartment,ClaimantShift,ClaimantEmploymentStartDate,RespondentID,RespondentName,RespondentUserID," +
            "RespondentManager,RespondentDepartment,RespondentShift,RespondentEmploymentStartDate,FirstMeetingDate,FirstMeetingOwner,FirstMeetingHRSupport,FirstMeetingOutcome,SecondMeetingDate,SecondMeetingOwner," +
            "SecondMeetingHRSupport,SecondMeetingOutcome, Paperless, IsUnionPresent,IsWIMRaised,ClosedAt,ClosedBy";
        

        public string GetDataRow()
        {
            return $"{MeetingStatus.VerifyCSV()},{ExactCaseID.VerifyCSV()},{MeetingType.VerifyCSV()},{ClaimantID.VerifyCSV()},{ClaimantName.VerifyCSV()},{ClaimantUserID.VerifyCSV()},{ClaimantManager.VerifyCSV()}," +
                $"{ClaimantDepartment.VerifyCSV()},{ClaimantShift.VerifyCSV()},{ClaimantEmploymentStartDate.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()},{RespondentID.VerifyCSV()},{RespondentName.VerifyCSV()}," +
                $"{RespondentUserID.VerifyCSV()},{RespondentManager.VerifyCSV()},{RespondentDepartment.VerifyCSV()},{RespondentShift.VerifyCSV()}," +
                $"{RespondentEmploymentStartDate.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()},{FirstMeetingDate.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()},{FirstMeetingOwner.VerifyCSV()}," +
                $"{FirstMeetingHRSupport.VerifyCSV()},{FirstMeetingOutcome.VerifyCSV()},{SecondMeetingDate.ToString(DataStorage.ShortPreviewDateFormat).VerifyCSV()},{SecondMeetingOwner.VerifyCSV()}," +
                $"{SecondMeetingHRSupport.VerifyCSV()},{SecondMeetingOutcome.VerifyCSV()},{Paperless},{IsUnionPresent},{IsWIMRaised},{ClosedAt.ToString(DataStorage.LongPreviewDateFormat).VerifyCSV()}," +
                $"{ClosedBy.VerifyCSV()}";
        }
    }
}
