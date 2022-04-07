namespace Domain.Models
{
    public class Settings
    {
        public string SiteID { get; set; }
        public string DbProductionPath { get; set; }
        public string LocalDbPath { get; set; }
        public string DbTestPath { get; set; }
        public int CacheRefreshRateInSeconds { get; set; }
        public string UserImgURL { get; set; }
        public string RosterURL { get; set; }
        public string PunchURL { get; set; }
        public string RosterWebDateFormat { get; set; }
        public string ResignationsQuicklinkURL { get; set; }

        public Settings()
        {
            SiteID = string.Empty;
            DbProductionPath = string.Empty;
            LocalDbPath = string.Empty;
            DbTestPath = string.Empty;
            UserImgURL = string.Empty;
            RosterURL = string.Empty;
            PunchURL = string.Empty;
            ResignationsQuicklinkURL = string.Empty;
            RosterWebDateFormat = "ddd MMM dd hh:mm:ss 'UTC' yyyy";
            CacheRefreshRateInSeconds = 30;
        }
    }
}
