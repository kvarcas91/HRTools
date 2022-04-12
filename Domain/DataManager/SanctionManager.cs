using Domain.Models.Sanctions;
using Domain.Types;
using System;
using System.Collections.Generic;

namespace Domain.DataManager
{
    public static class SanctionManager
    {
        public static DateTime GetSanctionEndDate(string sanction, DateTime sanctionStartDate)
        {
            switch(sanction)
            {
                case "1st LoC":
                case "Verbal Warning":
                    return sanctionStartDate.AddMonths(6);
                case "2nd LoC":
                case "Written Warning":
                    return sanctionStartDate.AddMonths(9);
                case "3rd LoC":
                case "Final Warning":
                    return sanctionStartDate.AddMonths(12);
                case "NFA":
                    return sanctionStartDate;
                default:
                    return sanctionStartDate;
            }
        }

        public static DateTime GetSanctionEndDate(SanctionEntity sanction) => GetSanctionEndDate(sanction.Sanction, sanction.SanctionStartDate);
        
        public static MeetingType GetMeetingType(string sanction)
        {
            switch (sanction)
            {
                case "1st LoC":
                case "2nd LoC":
                case "3rd LoC":
                    return MeetingType.Health;
                case "Verbal Warning":
                case "Written Warning":
                case "Final Warning":
                    return MeetingType.Disciplinary;
                default:
                    return MeetingType.Default;
            }
        }

        public static List<string> GetSanctions()
        {
            return new List<string> { "1st LoC", "2nd LoC", "3rd LoC", "Verbal Warning", "Written Warning", "Final Warning", "Termination" };
        }

        public static List<string> GetAwalSanctions()
        {
            return new List<string> {"Cancelled", "Verbal Warning", "Written Warning", "Final Warning", "Termination" };
        }
    }
}
