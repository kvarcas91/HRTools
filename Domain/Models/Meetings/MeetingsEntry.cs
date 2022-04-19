using System;

namespace Domain.Models.Meetings
{
    public class MeetingsEntry
    {
        public string ID { get; set; }
        public DateTime FirstMeetingDate { get; set; }
        public string MeetingType { get; set; }

        public bool CanAdd() => !string.IsNullOrEmpty(ID) && !string.IsNullOrEmpty(MeetingType) && FirstMeetingDate != DateTime.MinValue;
    }
}
