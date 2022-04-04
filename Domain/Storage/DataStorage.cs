using Domain.Models;
using System.Collections.Generic;

namespace Domain.Storage
{
    public static class DataStorage
    {
        public static Settings AppSettings;

        public static List<Roster> RosterList = new List<Roster>();

        //public static readonly string BaseAvatarPlaceholder = $@"{AppContext.BaseDirectory}Resources\Assets\person.png";

        public readonly static string AttendanceDateFormat = "M/d/yyyy h:mm ";

        public readonly static string LongDBDateFormat = "yyyy-MM-dd HH:mm:ss";
        public readonly static string ShortDBDateFormat = "yyyy-MM-dd";
        public readonly static string LongPreviewDateFormat = "dd/MM/yyyy HH:mm:ss";
        public readonly static string ShortPreviewDateFormat = "dd/MM/yyyy";
    }
}
