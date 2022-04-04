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
                                     (SELECT COUNT(*) FROM meetings where superseded = '0') as 'TotalERMeetingsCount',
                                     (SELECT COUNT(*) FROM meetings where meetingStatus in ('{CaseStatus.Open}','{CaseStatus.Pending}')) as 'OpenERMeetingsCount',
                                     (SELECT COUNT(*) FROM custom_meetings) as 'TotalMeetingsCount',
                                     (SELECT COUNT(*) FROM custom_meetings where meetingStatus in ('{CaseStatus.Open}','{CaseStatus.Pending}')) as 'OpenMeetingsCount',
                                     (SELECT COUNT(*) FROM sanctions where sanction <> 'NFA' AND overriden = '0') as TotalSanctionsCount,
                                     (SELECT COUNT(*) FROM sanctions where sanction <> 'NFA'  AND overriden = '0' AND sanctionEndDate > '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}%') as OpenSanctionsCount;";
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

        public  Task<bool> UpdateEmployeeStatusAsync (EmploymentStatus status, string emplId)
        {
            return Task.Run(async () =>
            {
                string query = "";
                Suspension? susp = await GetCachedScalarAsync<Suspension>($"SELECT * from suspensions WHERE employeeID = '{emplId}';");
                switch (status)
                {
                    case EmploymentStatus.Active:
                        
                        if (susp is null || !string.IsNullOrEmpty(susp.GetValueOrDefault().SuspensionRemovedBy)) return false;

                        var updatableSuspension = susp.GetValueOrDefault().SetResolver();
                        
                        query = $"UPDATE suspensions SET suspensionRemovedBy = '{updatableSuspension.SuspensionRemovedBy}', suspensionRemovedAt = '{updatableSuspension.SuspensionRemovedAt.ToString(DataStorage.LongDBDateFormat)}' WHERE id = '{updatableSuspension.ID}';";
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
                        
                        return await ExecuteAsync(query);
                    default:
                        return false;
                }
            });
        }

        public Task<EmployeeDataPreview> GetEmployeePreviewAsync(string emplId)
        {
            return Task.Run(() =>
            {
                string query = $@"SELECT (SELECT COUNT(*) FROM awal WHERE employeeID = '{emplId}') as 'TotalAWALCount',
                                     (SELECT COUNT(*) FROM meetings WHERE employeeID = '{emplId}') as 'TotalERMeetingsCount',
                                     (SELECT COUNT(*) FROM custom_meetings WHERE claimantID = '{emplId}' OR respondentID = '{emplId}') as 'TotalMeetingsCount',
                                     (SELECT COUNT(*) FROM sanctions where employeeID = '{emplId}') as TotalSanctionsCount;";
                return GetCachedScalarAsync<EmployeeDataPreview>(query);
            });
        }

        public Task<List<EmployeeSummary>> GetTimelineAsync(string emplId)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();

            return Task.Run(() =>
            {
                var sanctionList = GetCached<SanctionEntity>($"SELECT * FROM sanctions where employeeID = '{emplId}'").ToList();
                foreach (var item in sanctionList)
                {
                    list.AddRange(item.GetSummary());
                }

                var awalList = GetCached<AwalEntity>($"SELECT * FROM awal where employeeID = '{emplId}'").ToList();
                foreach (var item in awalList)
                {
                    list.AddRange(item.GetSummary());
                }

                return list.OrderByDescending(x => x.Date).ToList();
            });

            
        }
    }
}
