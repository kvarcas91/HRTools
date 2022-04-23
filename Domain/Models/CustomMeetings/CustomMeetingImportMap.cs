using Domain.Factory;
using System.Collections.Generic;

namespace Domain.Models.CustomMeetings
{
    public class CustomMeetingImportMap : IDataMap
    {
        public string ID { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string ClosedAt { get; set; }
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
        public string ClaimantEmploymentStartDate { get; set; }

        public string RespondentID { get; set; }
        public string RespondentName { get; set; }
        public string RespondentUserID { get; set; }
        public string RespondentManager { get; set; }
        public string RespondentDepartment { get; set; }
        public string RespondentShift { get; set; }
        public string RespondentEmploymentStartDate { get; set; }

        public string Paperless { get; set; }
        public string IsUnionPresent { get; set; }
        public string IsWIMRaised { get; set; }

        public string FirstMeetingDate { get; set; }
        public string FirstMeetingOwner { get; set; }
        public string FirstMeetingHRSupport { get; set; }
        public string FirstMeetingOutcome { get; set; }

        public string SecondMeetingDate { get; set; }
        public string SecondMeetingOwner { get; set; }
        public string SecondMeetingHRSupport { get; set; }
        public string SecondMeetingOutcome { get; set; }

        public CustomMeetingImportMap SetDefaultValues()
        {
            ID = "id";
            CreatedBy = "createdBy";
            CreatedAt = "createdAt";
            UpdatedBy = "updatedBy";
            UpdatedAt = "updatedAt";
            ClosedAt = "closedAt";
            ClosedBy = "closedBy";
            MeetingType = "meetingType";
            MeetingStatus = "meetingStatus";
            ExactCaseID = "exactCaseID";

            ClaimantID = "claimantID";
            ClaimantName = "claimantName";
            ClaimantUserID = "claimantUserID";
            ClaimantManager = "claimantManager";
            ClaimantShift = "claimantShift";
            ClaimantDepartment = "claimantDepartment";
            ClaimantEmploymentStartDate = "claimantEmploymentStartDate";

            RespondentID = "respondentID";
            RespondentName = "respondentName";
            RespondentUserID = "respondentUserID";
            RespondentManager = "respondentManager";
            RespondentShift = "respondentShift";
            RespondentDepartment = "respondentDepartment";
            RespondentEmploymentStartDate = "respondentEmploymentStartDate";

            Paperless = "paperless";
            IsWIMRaised = "isWIMRaised";
            IsUnionPresent = "isUnionPresent";

            FirstMeetingDate = "firstMeetingDate";
            FirstMeetingOwner = "firstMeetingOwner";
            FirstMeetingHRSupport = "firstMeetingHRSupport";
            FirstMeetingOutcome = "firstMeetingOutcome";

            SecondMeetingDate = "secondMeetingDate";
            SecondMeetingOwner = "secondMeetingOwner";
            SecondMeetingHRSupport = "secondMeetingHRSupport";
            SecondMeetingOutcome = "secondMeetingOutcome";
            
            return this;
        }

        public Dictionary<string, string> GetRequiredHeaders()
        {
            return new Dictionary<string, string>
            {
                ["ID"] = ID,
                ["CreatedBy"] = CreatedBy,
                ["CreatedAt"] = CreatedAt,
                ["UpdatedBy"] = UpdatedBy,
                ["UpdatedAt"] = UpdatedAt,
                ["ClosedAt"] = ClosedAt,
                ["ClosedBy"] = ClosedBy,
                ["MeetingType"] = MeetingType,
                ["MeetingStatus"] = MeetingStatus,
                ["ExactCaseID"] = ExactCaseID,

                ["ClaimantID"] = ClaimantID,
                ["ClaimantName"] = ClaimantName,
                ["ClaimantUserID"] = ClaimantUserID,
                ["ClaimantManager"] = ClaimantManager,
                ["ClaimantShift"] = ClaimantShift,
                ["ClaimantDepartment"] = ClaimantDepartment,
                ["ClaimantEmploymentStartDate"] = ClaimantEmploymentStartDate,

                ["RespondentID"] = RespondentID,
                ["RespondentName"] = RespondentName,
                ["RespondentUserID"] = RespondentUserID,
                ["RespondentManager"] = RespondentManager,
                ["RespondentShift"] = RespondentShift,
                ["RespondentDepartment"] = RespondentDepartment,
                ["RespondentEmploymentStartDate"] = RespondentEmploymentStartDate,

                ["Paperless"] = Paperless,
                ["IsWIMRaised"] = IsWIMRaised,
                ["IsUnionPresent"] = IsUnionPresent,

                ["FirstMeetingDate"] = FirstMeetingDate,
                ["FirstMeetingOwner"] = FirstMeetingOwner,
                ["FirstMeetingHRSupport"] = FirstMeetingHRSupport,
                ["FirstMeetingOutcome"] = FirstMeetingOutcome,

                ["SecondMeetingDate"] = SecondMeetingDate,
                ["SecondMeetingOwner"] = SecondMeetingOwner,
                ["SecondMeetingHRSupport"] = SecondMeetingHRSupport,
                ["SecondMeetingOutcome"] = SecondMeetingOutcome
            };
        }
    }
}
