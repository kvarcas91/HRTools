using Domain.Storage;
using Domain.Types;
using System;

namespace Domain.Models
{
    public struct Timeline
    {
        public string EmployeeID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EventMessage { get; set; }
        public string Origin { get; set; }

        public Timeline Create(string emplId, TimelineOrigin origin) 
        {
            EmployeeID = emplId;
            CreatedBy = Environment.UserName;
            CreatedAt = DateTime.Now;
            Origin = origin.ToString();
            return this;
        }

        public Timeline Create(string emplId, TimelineOrigin origin, string message)
        {
            EmployeeID = emplId;
            CreatedBy = Environment.UserName;
            CreatedAt = DateTime.Now;
            Origin = origin.ToString();
            EventMessage = message;
            return this;
        }

        public string GetHeader()
        {
            return "(employeeID, createdBy, createdAt, eventMessage, origin)";
        }

        public string GetValues()
        {
            return $"('{EmployeeID}','{CreatedBy}','{CreatedAt.ToString(DataStorage.LongDBDateFormat)}','{EventMessage}', '{Origin}')";
        }

    }
}
