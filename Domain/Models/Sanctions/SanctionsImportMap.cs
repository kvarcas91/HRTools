using Domain.Factory;
using System.Collections.Generic;

namespace Domain.Models.Sanctions
{
    public class SanctionsImportMap : IDataMap
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string Shift { get; set; }
        public string Manager { get; set; }
        public string MeetingType { get; set; }
        public string Sanction { get; set; }
        public string SanctionStartDate { get; set; }
        public string SanctionEndDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string OverridenBy { get; set; }
        public string OverridenAt { get; set; }

        public SanctionsImportMap SetDefaultValues()
        {

            ID = "id";
            EmployeeID = "employeeID";
            EmployeeName = "employeeName";
            UserID = "userID";
            Manager = "manager";
            Shift = "shift";
            Sanction = "sanction";
            SanctionStartDate = "sanctionStartDate";
            SanctionEndDate = "sanctionEndDate";
            CreatedBy = "createdBy";
            CreatedAt = "createdAt";
            MeetingType = "meetingType";
            OverridenAt = "overridenAt";
            OverridenBy = "overridenBy";

            return this;
        }

        public Dictionary<string, string> GetRequiredHeaders()
        {
            return new Dictionary<string, string>
            {
                ["EmployeeID"] = EmployeeID, 
                ["ID"] = ID, 
                ["UserID"] = UserID, 
                ["EmployeeName"] = EmployeeName, 
                ["Manager"] =  Manager, 
                ["Shift"] =  Shift,
                ["Sanction"] = Sanction,
                ["SanctionStartDate"] = SanctionStartDate,
                ["SanctionEndDate"] = SanctionEndDate,
                ["CreatedBy"] = CreatedBy,
                ["CreatedAt"] = CreatedAt,
                ["MeetingType"] = MeetingType,
                ["OverridenAt"] = OverridenAt,
                ["OverridenBy"] = OverridenBy
            };
        }
    }
}
