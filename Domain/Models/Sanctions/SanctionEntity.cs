using Domain.Types;
using System;

namespace Domain.Models.Sanctions
{
    public class SanctionEntity
    {
        public string ID { get; set; }
        public string EmployeeID { get; set; }
        public string UserID { get; set; }
        public string EmployeeName { get; set; }
        public string Shift { get; set; }
        public string Manager { get; set; }
        public string Sanction { get; set; }
        public DateTime SanctionStartDate { get; set; }
        public DateTime SanctionEndDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public MeetingType MeetingType { get; set; }
        public bool Overriden { get; set; }
        public string OverridenBy { get; set; }
        public DateTime OverridenAt { get; set; }
    }
}
