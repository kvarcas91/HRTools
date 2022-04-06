using Domain.Models;
using Domain.Models.AWAL;
using Domain.Models.DataSnips;
using Domain.Models.Sanctions;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class PreviewRepository : BaseRepository
    {
        public Task<DataPreview> GetPreviewAsync()
        {
            string query = $@"SELECT (SELECT COUNT(*) FROM awal) as 'TotalAWALCount', 
                                     (SELECT COUNT(*) FROM awal where awalStatus = '{(int)AwalStatus.Active}') as 'OpenAWALCount', 
                                     (SELECT COUNT(*) FROM resignations) as 'TotalResignationsCount',
                                     (SELECT COUNT(*) FROM meetings) as 'TotalERMeetingsCount',
                                     (SELECT COUNT(*) FROM meetings where meetingStatus in ('{CaseStatus.Open}','{CaseStatus.Pending}')) as 'OpenERMeetingsCount',
                                     (SELECT COUNT(*) FROM custom_meetings) as 'TotalMeetingsCount',
                                     (SELECT COUNT(*) FROM custom_meetings where meetingStatus in ('{CaseStatus.Open}','{CaseStatus.Pending}')) as 'OpenMeetingsCount',
                                     (SELECT COUNT(*) FROM sanctions where sanction <> 'NFA') as TotalSanctionsCount,
                                     (SELECT COUNT(*) FROM sanctions where sanction <> 'NFA' AND sanctionEndDate > '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}%') as OpenSanctionsCount;";
            return GetCachedScalarAsync<DataPreview>(query);
        }

        public Task<SanctionPreview> GetLiveSanctionsPreviewAsync(string emplId)
        {
            string query = $@"SELECT 
                            (SELECT sanction from sanctions where employeeID = '{emplId}' AND meetingType = '{(int)MeetingType.Health}' AND sanctionEndDate > '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}') as 'HealthSanction',
                            (SELECT sanctionEndDate from sanctions where employeeID = '{emplId}' AND meetingType = '{(int)MeetingType.Health}' AND sanctionEndDate > '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}') as 'HealthExpireDate',
                            (SELECT sanction from sanctions where employeeID = '{emplId}' AND meetingType = '{(int)MeetingType.Disciplinary}' AND sanctionEndDate > '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}') as 'DisciplinarySanction',
                            (SELECT sanctionEndDate from sanctions where employeeID = '{emplId}' AND meetingType = '{(int)MeetingType.Disciplinary}' AND sanctionEndDate > '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}') as 'DisciplinaryExpireDate'";
            return GetScalarAsync<SanctionPreview>(query);
        }

        public Task<EmployeeStatusSnip> GetEmployeeStatusSnipAsync(string emplId)
        {
            string query = $"SELECT count(*) as 'SuspensionCount' FROM suspensions WHERE employeeID = '{emplId}' AND (suspensionRemovedBy = '' OR suspensionRemovedAt LIKE '0001%');";

            return Task.Run(async () =>
            {
                var dbSnip = await GetCachedScalarAsync <EmployeeStatusSnip>(query);
                if (dbSnip.SuspensionCount == 0 && DataStorage.RosterList != null && DataStorage.RosterList.Count > 0)
                {
                    var isActiveOnRoster = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(emplId)).FirstOrDefault();
                    dbSnip.IsRosterActive = isActiveOnRoster != null;
                }
                return dbSnip;
            });
        }

        public  Task<Response> UpdateEmployeeStatusAsync (EmploymentStatus status, string emplId)
        {
            return Task.Run(async () =>
            {
                string query = "";
                var timelineEntry = new Timeline().Create(emplId);
                string timelineQuery = string.Empty;

                Suspension? susp = GetCachedScalar<Suspension>($"SELECT * from suspensions WHERE employeeID = '{emplId}';");
                switch (status)
                {
                    case EmploymentStatus.Active:
                        
                        if (susp is null || !string.IsNullOrEmpty(susp.GetValueOrDefault().SuspensionRemovedBy)) return new Response { Success = false };

                        var updatableSuspension = susp.GetValueOrDefault().SetResolver();

                        timelineEntry.EventMessage = $"AA has been activated by {Environment.UserName}";
                        timelineQuery = $"INSERT INTO timeline {timelineEntry.GetHeader()} VALUES {timelineEntry.GetValues()};";

                        query = $"UPDATE suspensions SET suspensionRemovedBy = '{updatableSuspension.SuspensionRemovedBy}', suspensionRemovedAt = '{updatableSuspension.SuspensionRemovedAt.ToString(DataStorage.LongDBDateFormat)}' WHERE id = '{updatableSuspension.ID}'; {timelineQuery}";
                        return await ExecuteAsync(query);

                    case EmploymentStatus.Suspended:
                        var sSquery = string.Empty;

                        if (susp is null || !string.IsNullOrEmpty(susp.GetValueOrDefault().SuspensionRemovedBy))
                        {
                            var uS = susp.GetValueOrDefault().SetCreator();
                            query = $"UPDATE suspensions SET createdAt = '{uS.CreatedAt.ToString(DataStorage.LongDBDateFormat)}', createdBy = '{uS.CreatedBy}', suspensionRemovedBy = '{uS.SuspensionRemovedBy}', suspensionRemovedAt = '{uS.SuspensionRemovedAt.ToString(DataStorage.LongDBDateFormat)}' WHERE id = '{uS.ID}';";
                        }
                        else
                        {
                            var s = new Suspension().SetId().SetCreator().SetEmployeeId(emplId);
                            query = $"INSERT INTO suspensions (id,employeeID,createdAt,createdBy,suspensionRemovedAt) VALUES ('{s.ID}', '{s.EmployeeID}', '{s.CreatedAt.ToString(DataStorage.LongDBDateFormat)}', '{s.CreatedBy}', '{s.SuspensionRemovedAt.ToString(DataStorage.LongDBDateFormat)}');";
                        }

                        timelineEntry.EventMessage = $"AA has been suspended by {Environment.UserName}";
                        timelineQuery = $"INSERT INTO timeline {timelineEntry.GetHeader()} VALUES {timelineEntry.GetValues()}; {timelineQuery}";
                        return await ExecuteAsync($"{query} {timelineQuery}");
                    default:
                        return new Response { Success = false };
                }
            });
        }

        public Task<EmployeeDataPreview> GetEmployeePreviewAsync(string emplId)
        {
            string query = $@"SELECT (SELECT COUNT(*) FROM awal WHERE employeeID = '{emplId}') as 'TotalAWALCount',
                                     (SELECT COUNT(*) FROM meetings WHERE employeeID = '{emplId}') as 'TotalERMeetingsCount',
                                     (SELECT COUNT(*) FROM custom_meetings WHERE claimantID = '{emplId}' OR respondentID = '{emplId}') as 'TotalMeetingsCount',
                                     (SELECT COUNT(*) FROM sanctions where employeeID = '{emplId}') as TotalSanctionsCount;";
            return GetCachedScalarAsync<EmployeeDataPreview>(query);
        }

        public Task<IEnumerable<Timeline>> GetTimelineAsync(string emplId)
        {
            var query = $"SELECT * FROM timeline WHERE employeeID = '{emplId}' ORDER BY createdAt DESC";
            return GetCachedAsync<Timeline>(query);
        }
    }
}
