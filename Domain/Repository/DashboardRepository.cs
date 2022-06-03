using Domain.Models.AWAL;
using Domain.Models.CustomMeetings;
using Domain.Models.DataSnips;
using Domain.Models.Meetings;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class DashboardRepository : BaseRepository
    {

        public Task<IEnumerable<int>> GetDistinctYears(uint tabIndex)
        {
            var table = GetTableNameFromTabIndex(tabIndex);
            var identifier = GetIdentifierFromTabIndex(tabIndex);
            string query = $"SELECT DISTINCT strftime('%Y',{identifier}) from {table} WHERE strftime('%YYYY',{identifier}) <> '' AND strftime('%Y',{identifier}) NOT NULL AND strftime('%Y',{identifier}) <> '0001'";
            return GetCachedAsync<int>(query);
        }

        public Task<MeetingMetrics> GetMeetingMetrics(DateTime firstWeekDate)
        {
            string startDate = firstWeekDate.AddDays(-1).ToString(DataStorage.ShortDBDateFormat);
            string endDate = firstWeekDate.AddDays(6).ToString(DataStorage.ShortDBDateFormat);

            string query = $@"SELECT
                            (SELECT COUNT(*) FROM meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND firstMeetingOutcome NOT IN ('NFA','Cancelled')) +
                            (SELECT COUNT(*) FROM meetings WHERE firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%') AS 'TotalCaseCount',
                            (SELECT COUNT(*) FROM meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome <> '')  +
                            (SELECT COUNT(*) FROM meetings WHERE firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%' AND firstMeetingOutcome <> '') AS 'CompletedMeetingsCount',
                            (SELECT COUNT(*) FROM meetings WHERE meetingStatus in ('Open', 'Pending') AND secondMeetingDate >= '{startDate}%' AND secondMeetingOutcome = '' AND secondMeetingDate <= '{endDate}%') +
                            (SELECT COUNT(*) FROM meetings WHERE meetingStatus in ('Open', 'Pending') AND firstMeetingOutcome = '' AND firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%') AS 'MissedMeetingsCount',
                            (SELECT COUNT(*) FROM meetings WHERE ((secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%') OR (firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%')) AND 
                            (firstMeetingOutcome = 'Cancelled' OR secondMeetingOutcome = 'Cancelled')) AS 'CancelledCount',
                            (SELECT COUNT(*) FROM meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome <> '' AND secondMeetingOutcome <> 'NFA') AS 'IssuedSanctionCount',
                            (SELECT COUNT(*) FROM meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome = 'NFA') +
                            (SELECT COUNT(*) FROM meetings WHERE firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%' AND firstMeetingOutcome = 'NFA') AS 'IssuedNFACount',
                            (SELECT COUNT(*) FROM meetings WHERE  ((secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%') OR (firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%')) AND 
                            meetingStatus = 'Pending') AS 'PendingCount',
                            (SELECT COUNT(*) FROM meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome = 'NFA') AS 'OverturnCount',
                            (SELECT COUNT(*) FROM meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome <> '' AND paperless = '1') +
                            (SELECT COUNT(*) FROM meetings WHERE firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%' AND firstMeetingOutcome <> '' AND paperless = '1') AS 'PaperlessCount'
                            ;";

            return GetCachedScalarAsync<MeetingMetrics>(query);
        }

        public Task<CustomMeetingMetrics> GetCustomMeetingMetrics(DateTime firstWeekDate)
        {
            string startDate = firstWeekDate.AddDays(-1).ToString(DataStorage.ShortDBDateFormat);
            string endDate = firstWeekDate.AddDays(6).ToString(DataStorage.ShortDBDateFormat);

            string query = $@"SELECT
                            (SELECT COUNT(*) FROM custom_meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND firstMeetingOutcome NOT IN ('NFA','Cancelled')) +
                            (SELECT COUNT(*) FROM custom_meetings WHERE firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%') AS 'TotalCaseCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome <> '')  +
                            (SELECT COUNT(*) FROM custom_meetings WHERE firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%' AND firstMeetingOutcome <> '') AS 'CompletedMeetingsCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE meetingStatus in ('Open', 'Pending') AND secondMeetingDate >= '{startDate}%' AND secondMeetingOutcome = '' AND secondMeetingDate <= '{endDate}%') +
                            (SELECT COUNT(*) FROM custom_meetings WHERE meetingStatus in ('Open', 'Pending') AND firstMeetingOutcome = '' AND firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%') AS 'MissedMeetingsCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE ((secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%') OR (firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%')) AND 
                            (firstMeetingOutcome = 'Cancelled' OR secondMeetingOutcome = 'Cancelled' OR meetingStatus = 'Cancelled')) AS 'CancelledCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome <> '' AND secondMeetingOutcome <> 'NFA') AS 'IssuedSanctionCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome = 'NFA') +
                            (SELECT COUNT(*) FROM custom_meetings WHERE firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%' AND firstMeetingOutcome = 'NFA') AS 'IssuedNFACount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE  ((secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%') OR (firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%')) AND 
                            meetingStatus = 'Pending') AS 'PendingCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome = 'NFA') AS 'OverturnCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE ((secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%') OR (firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%')) AND isWIMRaised = '1') AS 'WimRaisedCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE ((secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%') OR (firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%')) AND isUnionPresent = '1') AS 'UnionPresentCount',
                            (SELECT COUNT(*) FROM custom_meetings WHERE secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%' AND secondMeetingOutcome <> '' AND paperless = '1') +
                            (SELECT COUNT(*) FROM custom_meetings WHERE firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%' AND firstMeetingOutcome <> '' AND paperless = '1') AS 'PaperlessCount'
                            ;";

            return GetCachedScalarAsync<CustomMeetingMetrics>(query);
        }

        public Task<AwalMetrics> GetAwalMetrics(DateTime firstWeekDate)
        {
            string startDate = firstWeekDate.AddDays(-1).ToString(DataStorage.ShortDBDateFormat);
            string endDate = firstWeekDate.AddDays(6).ToString(DataStorage.ShortDBDateFormat);

            var query = $@"SELECT 
                            (SELECT COUNT(*) FROM awal WHERE firstNCNSDate >= '{startDate}%' AND firstNCNSDate <= '{endDate}%') as 'FirstNCNS',
                            (SELECT COUNT(*) FROM awal WHERE awal1SentDate >= '{startDate}%' AND awal1SentDate <= '{endDate}%') as 'AWAL1sent',
                            (SELECT COUNT(*) FROM awal WHERE awal2SentDate >= '{startDate}%' AND awal2SentDate <= '{endDate}%') as 'AWAL2sent',
                            (SELECT COUNT(*) FROM awal WHERE updatedAt >= '{startDate}%' AND updatedAt <= '{endDate}%' AND (awalStatus = '{(int)AwalStatus.Cancelled}' OR outcome = 'Cancelled')) as 'CancelledCount',
                            (SELECT COUNT(*) FROM awal WHERE disciplinaryDate >= '{startDate}%' AND disciplinaryDate <= '{endDate}%') as 'DisciplinaryMeetings';";
            return GetCachedScalarAsync<AwalMetrics>(query);
        }

        public Task<IEnumerable<MeetingsEntity>> GetSelectedMeetingsAsync(DateTime firstWeekDate)
        {
            string startDate = firstWeekDate.AddDays(-1).ToString(DataStorage.ShortDBDateFormat);
            string endDate = firstWeekDate.AddDays(6).ToString(DataStorage.ShortDBDateFormat);
            var query = $"SELECT * FROM meetings WHERE (secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%') OR (firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%')";

            return GetCachedAsync<MeetingsEntity>(query);
        }

        public Task<IEnumerable<CustomMeetingEntity>> GetSelectedCustomMeetingsAsync(DateTime firstWeekDate)
        {
            string startDate = firstWeekDate.AddDays(-1).ToString(DataStorage.ShortDBDateFormat);
            string endDate = firstWeekDate.AddDays(6).ToString(DataStorage.ShortDBDateFormat);
            var query = $"SELECT * FROM custom_meetings WHERE (secondMeetingDate >= '{startDate}%' AND secondMeetingDate <= '{endDate}%') OR (firstMeetingDate >= '{startDate}%' AND firstMeetingDate <= '{endDate}%')";

            return GetCachedAsync<CustomMeetingEntity>(query);
        }

        public Task<IEnumerable<AwalEntity>> GetSelectedAwalAsync(DateTime firstWeekDate)
        {
            string startDate = firstWeekDate.AddDays(-1).ToString(DataStorage.ShortDBDateFormat);
            string endDate = firstWeekDate.AddDays(6).ToString(DataStorage.ShortDBDateFormat);
            var query = $@"SELECT * FROM awal WHERE 
                        (firstNCNSDate >= '{startDate}%' AND firstNCNSDate <= '{endDate}%') OR 
                        (awal1SentDate >= '{startDate}%' AND awal1SentDate <= '{endDate}%') OR
                        (awal2SentDate >= '{startDate}%' AND awal2SentDate <= '{endDate}%') OR
                        (updatedAt >= '{startDate}%' AND updatedAt <= '{endDate}%' AND (awalStatus = '{(int)AwalStatus.Cancelled}' OR outcome = 'Cancelled')) OR
                        (disciplinaryDate >= '{startDate}%' AND disciplinaryDate <= '{endDate}%');";

            return GetCachedAsync<AwalEntity>(query);
        }

        #region Utility

        private string GetTableNameFromTabIndex(uint tabIndex)
        {
            switch (tabIndex)
            {
                case 0:
                    return "meetings";
                case 1:
                    return "custom_meetings";
                case 2:
                    return "awal";
                default:
                    throw new ArgumentException();
            }
        }

        private string GetIdentifierFromTabIndex(uint tabIndex)
        {
            switch (tabIndex)
            {
                case 0:
                    return "firstMeetingDate";
                case 1:
                    return "firstMeetingDate";
                case 2:
                    return "firstNCNSDate";
                default:
                    throw new ArgumentException();
            }
        }

        #endregion


    }
}
