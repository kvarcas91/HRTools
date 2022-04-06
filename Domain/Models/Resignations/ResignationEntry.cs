using System;

namespace Domain.Models.Resignations
{
    public class ResignationEntry
    {
        public string ReasonForResignation { get; set; }
        public string TTLink { get; set; }
        public DateTime LastWorkingDay { get; set; }

        public bool CanAdd()
        {
            return !string.IsNullOrEmpty(ReasonForResignation) && !string.IsNullOrEmpty(TTLink) && !LastWorkingDay.Equals(DateTime.MinValue);
        }
    }
}
