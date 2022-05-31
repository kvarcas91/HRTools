using Domain.Factory;
using Domain.Models.Meetings;
using Domain.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.DataManager
{
    public class ERMeetingsImportManager
    {
        public Task<(List<MeetingsEntity>, List<MeetingsEntity>)> GetERMeetingsAsync(IList<IDataImportObject> erMeetingList, IEnumerable<MeetingsEntity> allERMeetings)
        {
            return Task.Run(() =>
            {
                var updatableList = new List<MeetingsEntity>();
                var newMeetingList = new List<MeetingsEntity>();
                
                foreach (var item in erMeetingList)
                {
                    var erCase = item as ERMeetingImportMap;
                    if (erCase == null || string.IsNullOrEmpty(erCase.EmployeeID)) continue;

                    if (erCase.MeetingType == Types.MeetingType.Default || erCase.ReasonForContact.Contains("Unreported")) continue;
                    var meeting = allERMeetings.Where(x => x.ID.Equals(erCase.CaseNumber)).FirstOrDefault();
                    if (meeting == null)
                    {
                        var rosterEmpl = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(erCase.EmployeeID)).FirstOrDefault();
                        newMeetingList.Add(new MeetingsEntity(erCase, rosterEmpl));
                        continue;
                    }

                    var changesMade = false;
                    if ((erCase.Task.Contains("Informal") || erCase.Task.Contains("Investigation")) && meeting.FirstMeetingDate.Equals(DateTime.MinValue))
                    {
                        meeting.FirstMeetingDate = erCase.DueDate;
                        changesMade = true;
                    }
                    if ((erCase.Task.Contains("Formal") || erCase.Task.Contains("Disciplinary")) && meeting.SecondMeetingDate.Equals(DateTime.MinValue))
                    {
                        meeting.SecondMeetingDate = erCase.DueDate;
                        changesMade = true;
                    }
                    if (meeting.MeetingStatus.Equals("Closed") && !meeting.IsERCaseStatusOpen)
                    {
                        meeting.IsERCaseStatusOpen = true;
                        changesMade = true;
                    }

                    if (changesMade)
                    {
                        meeting.UpdatedAt = DateTime.Now;
                        meeting.UpdatedBy = "(er_automation)";
                        meeting.SetProgress();
                        updatableList.Add(meeting);
                    }
                }

                return (updatableList, newMeetingList);
            });
        }

        public Task<List<MeetingsEntity>> AlignAM(IEnumerable<MeetingsEntity> openMeetings)
        {
            return Task.Run(() =>
            {
                var updatableList = new List<MeetingsEntity>();
                foreach (var item in openMeetings)
                {
                    var rosterData = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(item.EmployeeID)).FirstOrDefault();
                    if (rosterData != null && (!rosterData.ShiftPattern.Equals(item.ShiftPattern) || !rosterData.ManagerName.Equals(item.ManagerName)))
                    {
                        item.ShiftPattern = rosterData.ShiftPattern;
                        item.ManagerName = rosterData.ManagerName;
                        item.EmployeeID = rosterData.EmployeeID;
                        item.DepartmentID = rosterData.DepartmentID;
                        updatableList.Add(item);
                    }
                }

                return updatableList;
            });

        }
    }
}
