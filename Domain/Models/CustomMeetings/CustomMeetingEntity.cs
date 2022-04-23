using Domain.Extensions;
using Domain.Factory;
using Domain.IO;
using Domain.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Models.CustomMeetings
{
    public class CustomMeetingEntity : IDataImportObject
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
       
        public ObservableCollection<CaseFile> RelatedFiles { get; private set; }
        public int FileCount { get; set; }
        public DateTime CaseAge { get; set; }

        public CustomMeetingEntity()
        {
            RelatedFiles = new ObservableCollection<CaseFile>();
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

        private void SetClaimant(Roster claimant)
        {
            if (claimant != null)
            {
                ClaimantID = claimant.EmployeeID;
                ClaimantName = claimant.EmployeeName;
                ClaimantManager = claimant.ManagerName;
                ClaimantDepartment = claimant.DepartmentID;
                ClaimantShift = claimant.ShiftPattern;
                ClaimantUserID = claimant.UserID;
                ClaimantEmploymentStartDate = claimant.EmploymentStartDate;
            }
            else
            {
                ClaimantName = string.Empty;
            }
        }

        private void SetRespondent(Roster respondent)
        {
            if (respondent != null)
            {
                RespondentID = respondent.EmployeeID;
                RespondentName = respondent.EmployeeName;
                RespondentManager = respondent.ManagerName;
                RespondentDepartment = respondent.DepartmentID;
                RespondentShift = respondent.ShiftPattern;
                RespondentUserID = respondent.UserID;
                RespondentEmploymentStartDate = respondent.EmploymentStartDate;
            }
            else
            {
                RespondentName = string.Empty;
            }
        }

        public async void SetFiles()
        {
            var fileList = await FileHelper.GetFolderContentFromMeetingID(ID);
            foreach (var item in fileList)
            {
                RelatedFiles.Add(item);
            }
        }

        public void SetAge()
        {
            DateTime endDate = ClosedAt.Equals(DateTime.MinValue) ? DateTime.Now : ClosedAt;
            CaseAge = DateTime.Now.AddDays(-(endDate - CreatedAt).Days);
        }
    }
}
