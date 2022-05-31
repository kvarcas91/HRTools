using Domain.Factory;
using Domain.Types;
using System;
using System.Collections.Generic;

namespace Domain.Models.Meetings
{
    public class ERMeetingImportMap : IDataMap, IDataImportObject
    {
        public string CaseNumber { get; set; }
        public string Task { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string EmployeeID { get; set; }
        public string ReasonForContact { get; set; }
        public MeetingType MeetingType { get; set; }

        public string GetHeader()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetRequiredHeaders()
        {
            return new Dictionary<string, string>()
            {
                ["CaseNumber"] = "casenumber",
                ["ReasonForContact"] = "reasonforcontact",
                ["Task"] = "task",
                ["DueDate"] = "duedate",
                ["EmployeeID"] = "emplid"
            };
        }

        public string GetValues()
        {
            throw new NotImplementedException();
        }

        public object ReadFromCSV(string[] fields, DataMap dataMap)
        {

            CaseNumber = dataMap.GetStrValue(nameof(CaseNumber), fields);
            Task = dataMap.GetStrValue(nameof(Task), fields);
            DueDate = dataMap.GetDateValue(nameof(DueDate), fields);
            Status = "Open";
            EmployeeID = dataMap.GetStrValue(nameof(EmployeeID), fields);
            ReasonForContact = dataMap.GetStrValue(nameof(ReasonForContact), fields);
            SetMeetingType();

            return this;
        }

        private void SetMeetingType()
        {
            if (string.IsNullOrEmpty(ReasonForContact)) return;
            if (ReasonForContact.Contains("Long Term Sickness")) return;

            if (!ReasonForContact.Contains("Long Term Sickness") && Task.Contains("Health"))
            {
                MeetingType = MeetingType.Health;
                return;
            }

            if (!ReasonForContact.Contains("Long Term Sickness") && Task.Contains("Meeting"))
            {
                MeetingType = MeetingType.Disciplinary;
                return;
            }
        }
    }
}
