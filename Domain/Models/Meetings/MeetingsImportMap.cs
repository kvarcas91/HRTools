using Domain.Factory;
using System.Collections.Generic;

namespace Domain.Models.Meetings
{
    public class MeetingsImportMap : IDataMap
    {

        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string ShiftPattern { get; set; }
        public string ManagerName { get; set; }
        public string MeetingType { get; set; }
        public string FirstMeetingDate { get; set; }
        public string FirstMeetingOutcome { get; set; }
        public string SecondMeetingDate { get; set; }
        public string SecondMeetingOutcome { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedAt { get; set; }
        public string MeetingStatus { get; set; }
        public string IsERCaseStatusOpen { get; set; }
        public string Paperless { get; set; }


        public MeetingsImportMap SetDefaultValues()
        {

            ID = "id";
            EmployeeID = "employeeID";
            EmployeeName = "employeeName";
            UserID = "userID";
            ManagerName = "manager";
            ShiftPattern = "shift";
            FirstMeetingDate = "firstMeetingDate";
            FirstMeetingOutcome = "firstMeetingOutcome";
            SecondMeetingDate = "secondMeetingDate";
            SecondMeetingOutcome = "secondMeetingOutcome";
            CreatedBy = "createdBy";
            CreatedAt = "createdAt";
            UpdatedBy = "updatedBy";
            UpdatedAt = "updatedAt";
            IsERCaseStatusOpen = "isERCaseStatusOpen";
            Paperless = "paperless";
            MeetingType = "meetingType";
            MeetingStatus = "meetingStatus";

            return this;
        }

        public Dictionary<string, string> GetRequiredHeaders()
        {
            return new Dictionary<string, string>
            {
                {"EmployeeID", EmployeeID }, {"ID", ID },{"UserID", UserID },{ "EmployeeName", EmployeeName},  { "ManagerName", ManagerName}, { "ShiftPattern", ShiftPattern}, 
                {"FirstMeetingDate", FirstMeetingDate }, { "FirstMeetingOutcome", FirstMeetingOutcome}, { "SecondMeetingDate", SecondMeetingDate}, {"SecondMeetingOutcome", SecondMeetingOutcome },
                { "CreatedBy", CreatedBy},{ "CreatedAt", CreatedAt}, {"UpdatedBy", UpdatedBy },{ "UpdatedAt", UpdatedAt},{ "IsERCaseStatusOpen", IsERCaseStatusOpen},
                { "Paperless", Paperless}, { "MeetingType", MeetingType},{ "MeetingStatus", MeetingStatus}
            };
        }
    }
}
