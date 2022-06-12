using Domain.Factory;
using System.Collections.Generic;

namespace Domain.Models.Resignations
{
    public class ResignationImportMap : IDataMap
    {

        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public string Shift { get; set; }
        public string EmploymentStartDate { get; set; }
        public string LastWorkingDay { get; set; }
        public string TTLink { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string ReasonForResignation { get; set; }

        public ResignationImportMap SetDefaultValues()
        {

            ID = "id";
            EmployeeID = "employeeID";
            Name = "name";
            UserID = "userID";
            Manager = "manager";
            Shift = "shift";
            EmploymentStartDate = "employmentStartDate";
            LastWorkingDay = "lastWorkingDay";
            TTLink = "ttLink";
            CreatedBy = "createdBy";
            CreatedAt = "createdAt";
            ReasonForResignation = "reasonForResignation";

            return this;
        }

        public Dictionary<string, string> GetRequiredHeaders()
        {
            return new Dictionary<string, string>
            {
                ["EmployeeID"] = EmployeeID,
                ["ID"] = ID,
                ["UserID"] = UserID,
                ["Name"] = Name,
                ["Manager"] = Manager,
                ["Shift"] = Shift,
                ["EmploymentStartDate"] = EmploymentStartDate,
                ["LastWorkingDay"] = LastWorkingDay,
                ["TTLink"] = TTLink,
                ["CreatedBy"] = CreatedBy,
                ["CreatedAt"] = CreatedAt,
                ["ReasonForResignation"] = ReasonForResignation
            };
        }
    }
}
