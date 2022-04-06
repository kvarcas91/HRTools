using Domain.Storage;
using System;

namespace Domain.Models
{
    public struct Timeline
    {
        public string EmployeeID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EventMessage { get; set; }

        public Timeline Create(string emplId) 
        {
            EmployeeID = emplId;
            CreatedBy = Environment.UserName;
            CreatedAt = DateTime.Now;
            return this;
        }

        public string GetHeader()
        {
            return "(employeeID, createdBy, createdAt, eventMessage)";
        }

        public string GetValues()
        {
            return $"('{EmployeeID}','{CreatedBy}','{CreatedAt.ToString(DataStorage.LongDBDateFormat)}','{EventMessage}')";
        }

    }
}
