using System;

namespace Domain.Models
{
    public struct Suspension
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime SuspensionRemovedAt { get; set; }
        public string SuspensionRemovedBy { get; set; }

        public Suspension SetId()
        {
            ID = Guid.NewGuid().ToString();
            return this;
        }

        public Suspension SetEmployeeId(string emplId)
        {
            EmployeeID = emplId;
            return this;
        }

        public Suspension SetCreator()
        {
            CreatedAt = DateTime.Now;
            CreatedBy = Environment.UserName;
            SuspensionRemovedBy = string.Empty;
            SuspensionRemovedAt = DateTime.MinValue;
            return this;
        }

        public Suspension SetResolver()
        {
            SuspensionRemovedAt = DateTime.Now;
            SuspensionRemovedBy = Environment.UserName;
            return this;
        }
    }
}
