using Domain.Automation;
using Domain.DataValidation;
using Domain.DataValidation.Meetings;
using Domain.Extensions;
using Domain.Factory;
using Domain.Models;
using Domain.Models.CustomMeetings;
using Domain.Models.Meetings;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class MeetingsRepository : BaseRepository
    {

        private readonly IDataValidation _validator;

        public MeetingsRepository()
        {
            _validator = new MeetingsValidation();
        }

        public Task<Response> InsertAllAsync(IList<IDataImportObject> meetingList) => base.InsertAllAsync(meetingList, "meetings");

        public Task<Response> UpdateAllAsync(IList<MeetingsEntity> meetingsList)
        {
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var item in meetingsList)
            {
                queryBuilder.Append($@"UPDATE meetings SET meetingStatus = '{item.MeetingStatus}', firstMeetingDate = {item.FirstMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, 
                                    secondMeetingDate = {item.SecondMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, 
                                    shiftPattern = '{item.ShiftPattern}', managerName = '{item.ManagerName.DbSanityCheck()}', employeeName = '{item.EmployeeName.DbSanityCheck()}', departmentID = '{item.DepartmentID}',
                                    isERCaseStatusOpen = '{Convert.ToInt32(item.IsERCaseStatusOpen)}' WHERE id = '{item.ID}';");
            }

            return ExecuteAsync(queryBuilder.ToString());
        }

        public Task<Response> InsertAllAsync(IList<MeetingsEntity> meetingList) => base.InsertAllAsync(meetingList, "meetings");

        public Task<Response> InsertCustomAllAsync(IList<IDataImportObject> meetingList) => base.InsertAllAsync(meetingList, "custom_meetings");

        public Task<IEnumerable<MeetingsEntity>> GetEmployeeMeetingsAsync(string emplId) => 
            GetCachedAsync<MeetingsEntity>($"SELECT * FROM meetings WHERE employeeID = '{emplId}' ORDER BY createdAt DESC");

        public Task<IEnumerable<MeetingsEntity>> GetMeetingsAsync() => GetCachedAsync<MeetingsEntity>($"SELECT * FROM meetings;");

        public Task<IEnumerable<CustomMeetingEntity>> GetCustomMeetingsAsync() => GetCachedAsync<CustomMeetingEntity>($"SELECT * FROM custom_meetings;");

        public Task<IEnumerable<MeetingsEntity>> GetMeetingsAsync(string selectedMeetingStatus, string selectedManager, string selectedMeetingType)
        {
            var query = string.IsNullOrEmpty(selectedMeetingStatus) || selectedMeetingStatus.Equals("Open/Pending") ? "('Open', 'Pending')" : $"('{selectedMeetingStatus}')";
            var managerQuery = string.IsNullOrEmpty(selectedManager) || selectedManager.Equals("All") ? "" : $"AND managerName LIKE '%{selectedManager}%'";
            var conj = string.IsNullOrEmpty(managerQuery) ? "AND" : "";

            var meetingTypeQuery = selectedMeetingType.Equals("All") ? "" : $"{conj} ((firstMeetingDate < '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}' AND secondMeetingDate is NULL) OR (secondMeetingDate < '{DateTime.Now.ToString(DataStorage.ShortDBDateFormat)}'))";

            return GetCachedAsync<MeetingsEntity>($"SELECT * FROM meetings WHERE meetingStatus in {query} {managerQuery} {meetingTypeQuery} ORDER BY createdAt DESC;");
        }

        public Task<IEnumerable<CustomMeetingEntity>> GetCustomMeetingsAsync(string selectedMeetingStatus, string selectedHRSupport, string meetingType)
        {
            var query = string.IsNullOrEmpty(selectedMeetingStatus) || selectedMeetingStatus.Equals("Open/Pending") ? "('Open', 'Pending')" : $"('{selectedMeetingStatus}')";
            var selectedHRSupportQuery = selectedHRSupport.Equals("All") ? "" : $"AND (firstMeetingHRSupport LIKE '%{selectedHRSupport}%' OR secondMeetingHRSupport LIKE '%{selectedHRSupport}%')";
            var meetingTypeQuery = meetingType.Equals("All") ? "" : $"AND meetingType = '{meetingType}'";

            return GetCachedAsync<CustomMeetingEntity>($"SELECT * FROM custom_meetings WHERE meetingStatus in {query} {selectedHRSupportQuery} {meetingTypeQuery} ORDER BY createdAt DESC;");
        }

        public Task<IEnumerable<string>> GetMeetingsDistinctManagersAsync() => GetCachedAsync<string>("SELECT DISTINCT managerName FROM meetings WHERE managerName NOT LIKE '' ORDER BY managerName;");

        public Task<IEnumerable<string>> GetCustomMeetingsDistinctHRSupportAsync() =>
            GetCachedAsync<string>("select firstMeetingHRSupport from custom_meetings where firstMeetingHRSupport not like '' union select secondMeetingHRSupport from custom_meetings where secondMeetingHRSupport not like '';");

        public Task<IEnumerable<MeetingsEntity>> GetOutstandingMeetingsAsync()
        {
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek;
            var date = DateTime.Now.AddDays(6 - currentDayOfWeek);
            string meetingDeadlineDate =  date.AddDays(1).ToString("yyyy-MM-dd");

            string query = $@"select * from meetings where meetingStatus in ('Open', 'Pending') AND meetingType IN (1,2) AND 
                                ((firstMeetingDate NOT NULL AND firstMeetingDate < '{meetingDeadlineDate}' AND secondMeetingDate IS NULL AND firstMeetingOutcome = '') OR
                                (secondMeetingDate NOT NULL AND secondMeetingDate < '{meetingDeadlineDate}' AND secondMeetingOutcome = ''))";

            return GetCachedAsync<MeetingsEntity>(query);
        }

        public Task<IEnumerable<CustomMeetingEntity>> GetEmployeeCustomMeetingsAsync(string emplId) =>
            GetCachedAsync<CustomMeetingEntity>($"SELECT * FROM custom_meetings WHERE claimantID = '{emplId}' OR respondentID = '{emplId}' ORDER BY createdAt DESC");

        public Task<Response> InsertAsync(MeetingsEntity meeting)
        {
            if (IsErMeetingExists(meeting.ID)) return Task.Run(() => new Response { Success = false, Message = "ER case with this ID already exists" });

            var timeLine = new Timeline().Create(meeting.EmployeeID, TimelineOrigin.Meetings, $"ER meeting (type - {meeting.MeetingType}) has been created by {meeting.CreatedBy}. Case ID: {meeting.ID}");
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";

            var query = $"INSERT INTO meetings {meeting.GetHeader()} VALUES {meeting.GetValues()}; {tlQuery}";
            return ExecuteAsync(query);
        }

        public Task<Response> InsertCustomAsync(CustomMeetingEntity meeting)
        {
            var tlQueryBuilder = new StringBuilder("INSERT INTO timeline ");
            if (!string.IsNullOrEmpty(meeting.ClaimantName))
            {
                var timeLine = new Timeline().Create(meeting.ClaimantID, TimelineOrigin.CustomMeetings, $"{meeting.MeetingType} meeting has been created by {meeting.CreatedBy}");
                tlQueryBuilder.Append($"{timeLine.GetHeader()} VALUES {timeLine.GetValues()}");
            }
            if (!string.IsNullOrEmpty(meeting.RespondentName))
            {
                var timeLine = new Timeline().Create(meeting.RespondentID, TimelineOrigin.CustomMeetings, $"{meeting.MeetingType} meeting has been created by {meeting.CreatedBy}");
                if (string.IsNullOrEmpty(meeting.ClaimantName)) tlQueryBuilder.Append($"{timeLine.GetHeader()} VALUES ");
                else tlQueryBuilder.Append(",");
                tlQueryBuilder.Append(timeLine.GetValues());
            }

            tlQueryBuilder.Append(";");

            var query = $"INSERT INTO custom_meetings {meeting.GetHeader()} VALUES {meeting.GetValues()}; {tlQueryBuilder.ToString()}";
            return ExecuteAsync(query);
        }

        private bool IsErMeetingExists(string id)
        {
            return GetScalar<int>($"SELECT COUNT(*) FROM meetings WHERE id = '{id}';") > 0;
        }

        public Task<Response> CloseERMeeting(MeetingsEntity meeting, string closureReason)
        {
            var timeLine = new Timeline().Create(meeting.EmployeeID, TimelineOrigin.Meetings, $" ER meeting (ID: {meeting.ID}) has been cancelled by {Environment.UserName} due to '{closureReason}'");
            if (meeting.SecondMeetingDate != DateTime.MinValue) meeting.SecondMeetingOutcome = "Cancelled";
            if (meeting.SecondMeetingDate == DateTime.MinValue) meeting.FirstMeetingOutcome = "Cancelled";
            meeting.MeetingStatus = "Cancelled";
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";
            var query = $@"UPDATE meetings SET meetingStatus = '{meeting.MeetingStatus}', firstMeetingOutcome = '{meeting.FirstMeetingOutcome}', SecondMeetingOutcome = '{meeting.SecondMeetingOutcome}', 
            updatedBy = '{meeting.UpdatedBy}', updatedAt = {meeting.UpdatedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)} WHERE id = '{meeting.ID}'; {tlQuery}";
            return ExecuteAsync(query);
        }

        public Task<Response> CloseCustomMeeting(CustomMeetingEntity meeting, string closureReason)
        {
            var emplId = string.Empty;
            if (string.IsNullOrEmpty(meeting.ClaimantID) && !string.IsNullOrEmpty(meeting.RespondentID)) emplId = meeting.RespondentID;
            if (string.IsNullOrEmpty(meeting.RespondentID) && !string.IsNullOrEmpty(meeting.ClaimantID)) emplId = meeting.ClaimantID;

            var timeLine2 = new Timeline();
            var tl2Query = string.Empty;
            if (string.IsNullOrEmpty(emplId))
            {
                emplId = meeting.ClaimantID;
                timeLine2 = timeLine2.Create(meeting.RespondentID, TimelineOrigin.Meetings, $"{meeting.MeetingType} meeting has been cancelled by {Environment.UserName} due to '{closureReason}'");
                tl2Query = $",{timeLine2.GetValues()}";
            }
            var timeLine = new Timeline().Create(emplId, TimelineOrigin.Meetings, $"{meeting.MeetingType} meeting has been cancelled by {Environment.UserName} due to '{closureReason}'");
            if (meeting.SecondMeetingDate != DateTime.MinValue) meeting.SecondMeetingOutcome = "Cancelled";
            if (meeting.SecondMeetingDate == DateTime.MinValue) meeting.FirstMeetingOutcome = "Cancelled";

            meeting.MeetingStatus = "Cancelled";
            meeting.ClosedAt = DateTime.Now;
            meeting.ClosedBy = Environment.UserName;

            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()}{tl2Query};";
            var query = $@"UPDATE custom_meetings SET meetingStatus = '{meeting.MeetingStatus}', firstMeetingOutcome = '{meeting.FirstMeetingOutcome}', SecondMeetingOutcome = '{meeting.SecondMeetingOutcome}', 
            closedBy = '{meeting.ClosedBy}', closedAt = {meeting.ClosedAt.DbNullableSanityCheck(DataStorage.LongDBDateFormat)} WHERE id = '{meeting.ID}'; {tlQuery}";
            return ExecuteAsync(query);
        }

        public Task<Response> ChangeMeetingStatusAsync(MeetingsEntity meeting, string status)
        {
            var timeLine = new Timeline().Create(meeting.EmployeeID, TimelineOrigin.Meetings, $"Meeting status (meeting ID: '{meeting.ID}') has been changed from '{meeting.MeetingStatus}' to '{status}' by {Environment.UserName}");
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";

            meeting.MeetingStatus = status;

            var query = $"UPDATE meetings SET meetingStatus = '{status}' WHERE id = '{meeting.ID}'; {tlQuery}";
            return ExecuteAsync(query);
        }

        public Task<Response> UpdateAsync(MeetingsEntity meeting)
        {

            var validationResponse = _validator.Validate(meeting);
            if (!validationResponse.Success) return Task.Run(() => validationResponse);

            var dbMeeting = GetScalar<MeetingsEntity>($"SELECT * FROM meetings WHERE id = '{meeting.ID}';");
            var timelineQuery = GetUpdateTimelineString(meeting, dbMeeting);
            if (string.IsNullOrEmpty(timelineQuery) && dbMeeting.Paperless == meeting.Paperless) return Task.Run(() => new Response { Success = false, Message = "No changes were made" });

            meeting.UpdatedAt = DateTime.Now;
            meeting.UpdatedBy = Environment.UserName;

            if ((!string.IsNullOrEmpty(meeting.FirstMeetingOutcome) && meeting.FirstMeetingOutcome == "NFA") || !string.IsNullOrEmpty(meeting.SecondMeetingOutcome)) meeting.MeetingStatus = "Closed";

            var query = $@"UPDATE meetings SET firstMeetingDate = {meeting.FirstMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, firstMeetingOutcome = '{meeting.FirstMeetingOutcome}', 
                        secondMeetingDate = {meeting.SecondMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, secondMeetingOutcome = '{meeting.SecondMeetingOutcome}', updatedBy = '{Environment.UserName}', 
                        updatedAt = '{meeting.UpdatedAt.ToString(DataStorage.LongDBDateFormat)}', meetingStatus = '{meeting.MeetingStatus}', paperless = '{Convert.ToInt16(meeting.Paperless)}',
                        rtwDate = {meeting.RTWDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}
                        WHERE id = '{meeting.ID}'; {timelineQuery}";

           
            Automate(meeting, dbMeeting, AutomationAction.OnUpdate);
            return ExecuteAsync(query);
        }

        public Task<Response> UpdateCustomAsync (CustomMeetingEntity meeting)
        {
            var validationResponse = _validator.Validate(meeting);
            if (!validationResponse.Success) return Task.Run(() => validationResponse);

            var dbMeeting = GetScalar<CustomMeetingEntity>($"SELECT * FROM custom_meetings WHERE id = '{meeting.ID}';");
            var timelineQuery = GetCustomUpdateTimelineString(meeting, dbMeeting);
            if (string.IsNullOrEmpty(timelineQuery) && dbMeeting.Paperless == meeting.Paperless && dbMeeting.IsUnionPresent == meeting.IsUnionPresent && dbMeeting.IsWIMRaised == meeting.IsWIMRaised) 
                    return Task.Run(() => new Response { Success = false, Message = "No changes were made" });

            meeting.UpdatedAt = DateTime.Now;
            meeting.UpdatedBy = Environment.UserName;

            if ((!string.IsNullOrEmpty(meeting.FirstMeetingOutcome) && (meeting.FirstMeetingOutcome == "NFA" || meeting.SecondMeetingOutcomeList.Count == 0)) || !string.IsNullOrEmpty(meeting.SecondMeetingOutcome)) meeting.MeetingStatus = "Closed";
            var query = $@"UPDATE custom_meetings SET firstMeetingDate = {meeting.FirstMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, secondMeetingDate = {meeting.SecondMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)},
                        firstMeetingOutcome = '{meeting.FirstMeetingOutcome}', secondMeetingOutcome = '{meeting.SecondMeetingOutcome}', 
                        firstMeetingOwner = '{meeting.FirstMeetingOwner.Trim().DbSanityCheck()}', secondMeetingOwner = '{meeting.SecondMeetingOwner.Trim().DbSanityCheck()}',
                        firstMeetingHRSupport = '{meeting.FirstMeetingHRSupport.Trim().DbSanityCheck()}', secondMeetingHRSupport = '{meeting.SecondMeetingHRSupport.Trim().DbSanityCheck()}',
                        updatedBy = '{Environment.UserName}', updatedAt = '{meeting.UpdatedAt.ToString(DataStorage.LongDBDateFormat)}', meetingStatus = '{meeting.MeetingStatus}', 
                        paperless = '{Convert.ToInt16(meeting.Paperless)}', isUnionPresent = '{Convert.ToInt16(meeting.IsUnionPresent)}', isWIMRaised = '{Convert.ToInt16(meeting.IsWIMRaised)}' 
                        WHERE id = '{meeting.ID}'; {timelineQuery}";

            AutomateCustom(meeting, dbMeeting, AutomationAction.OnUpdate);
            return ExecuteAsync(query);
        }

        private string GetUpdateTimelineString(MeetingsEntity meetitng, MeetingsEntity dbObj)
        {
            if (dbObj == null) return string.Empty;
            var haveUpdate = false;
            var timelineString = new StringBuilder("INSERT INTO timeline ");
            
            if (meetitng.FirstMeetingDate != dbObj.FirstMeetingDate)
            {
                var tl = new Timeline().Create(meetitng.EmployeeID, TimelineOrigin.Meetings, $"ER meeting (ID: {meetitng.ID}) first meeting date has been updated by {Environment.UserName}. Changed '{dbObj.FirstMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}' into '{meetitng.FirstMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}'");
                timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                haveUpdate = true;
            }

            if (meetitng.FirstMeetingOutcome != dbObj.FirstMeetingOutcome)
            {
                var message = string.IsNullOrEmpty(dbObj.FirstMeetingOutcome) ? $"ER meeting (ID: {meetitng.ID}) first meeting outcome ({meetitng.FirstMeetingOutcome}) has been recorded by {Environment.UserName}" : 
                    $"ER meeting (ID: {meetitng.ID}) first meeting outcome has been updated by {Environment.UserName}. Changed '{dbObj.FirstMeetingOutcome}' into '{meetitng.FirstMeetingOutcome}'";

                var tl = new Timeline().Create(meetitng.EmployeeID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                    haveUpdate = true;
                }
                else timelineString.Append($",{tl.GetValues()}");
            }

            if (meetitng.SecondMeetingDate != dbObj.SecondMeetingDate)
            {
                var message = dbObj.SecondMeetingDate == DateTime.MinValue ? $"ER meeting (ID: {meetitng.ID}) second meeting date ({meetitng.SecondMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}) has been recorded by {Environment.UserName}" :
                    meetitng.SecondMeetingDate == DateTime.MinValue ? $"second meeting date has been removed by {Environment.UserName}" :
                    $"ER meeting (ID: {meetitng.ID}) second meeting date has been updated by {Environment.UserName}. Changed '{dbObj.SecondMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}' into '{meetitng.SecondMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}'";
                var tl = new Timeline().Create(meetitng.EmployeeID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                    haveUpdate = true;
                }
                else timelineString.Append($",{tl.GetValues()}");
            }

            if (meetitng.SecondMeetingOutcome != dbObj.SecondMeetingOutcome)
            {
                var message = string.IsNullOrEmpty(dbObj.SecondMeetingOutcome) ? $"ER meeting (ID: {meetitng.ID}) second meeting outcome ({meetitng.SecondMeetingOutcome}) has been recorded by {Environment.UserName}" :
                    $"ER meeting (ID: {meetitng.ID}) second meeting outcome has been updated by {Environment.UserName}. Changed '{dbObj.SecondMeetingOutcome}' into '{meetitng.SecondMeetingOutcome}'";

                var tl = new Timeline().Create(meetitng.EmployeeID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                    haveUpdate = true;
                }
                else timelineString.Append($",{tl.GetValues()}");
            }

            if (meetitng.RTWDate != dbObj.RTWDate)
            {
                var message = dbObj.RTWDate == DateTime.MinValue ? $"RTW date ({meetitng.RTWDate.ToString(DataStorage.ShortPreviewDateFormat)}) has been recorded by {Environment.UserName}" : 
                    $"RTW date has been changed from '{dbObj.RTWDate.ToString(DataStorage.ShortPreviewDateFormat)}' to '{meetitng.RTWDate.ToString(DataStorage.ShortPreviewDateFormat)}'";

                var tl = new Timeline().Create(meetitng.EmployeeID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    timelineString.Append($"{tl.GetHeader()} VALUES {tl.GetValues()}");
                    haveUpdate = true;
                }
                else timelineString.Append($",{tl.GetValues()}");
            }

            return haveUpdate ? timelineString.ToString() : string.Empty;
        }

        private string GetCustomUpdateTimelineString (CustomMeetingEntity meeting, CustomMeetingEntity dbObj)
        {
            if (dbObj == null) return string.Empty;
            var haveUpdate = false;
            var timelineString = new StringBuilder("INSERT INTO timeline ");

            if (meeting.FirstMeetingDate != dbObj.FirstMeetingDate)
            {
                var tl2 = new Timeline().Create(meeting.RespondentID, TimelineOrigin.Meetings, $"{meeting.MeetingType} meeting first meeting date has been updated by {Environment.UserName}. Changed '{dbObj.FirstMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}' into '{meeting.FirstMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}'");

                if (tl2.EmployeeID != null)
                {
                    timelineString.Append($"{tl2.GetHeader()} VALUES {tl2.GetValues()}");
                }
                haveUpdate = true;
            }
            if (meeting.SecondMeetingDate != dbObj.SecondMeetingDate)
            {
                var message = dbObj.SecondMeetingDate == DateTime.MinValue ? $"{meeting.MeetingType} meeting second meeting date ({meeting.SecondMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}) has been recorded by {Environment.UserName}" :
                    meeting.SecondMeetingDate == DateTime.MinValue ? $"{meeting.MeetingType} meeting second meeting date has been removed by {Environment.UserName}" :
                   $"{meeting.MeetingType} meeting second meeting date has been updated by {Environment.UserName}. Changed '{dbObj.SecondMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}' into '{meeting.SecondMeetingDate.ToString(DataStorage.ShortPreviewDateFormat)}'";

                var tl2 = new Timeline().Create(meeting.RespondentID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    if (tl2.EmployeeID != null) timelineString.Append($"{tl2.GetHeader()} VALUES {tl2.GetValues()}");
                    haveUpdate = true;
                }
                else
                {
                    if (tl2.EmployeeID != null) timelineString.Append($",{tl2.GetValues()}");
                }

            }

            if (meeting.FirstMeetingOwner != dbObj.FirstMeetingOwner)
            {
                var message = string.IsNullOrEmpty(dbObj.FirstMeetingOwner) && !string.IsNullOrEmpty(meeting.FirstMeetingOwner) ? $"{meeting.MeetingType} meeting first meeting owner ({meeting.FirstMeetingOwner}) has been recorded by {Environment.UserName}" :
                    string.IsNullOrEmpty(meeting.FirstMeetingOwner) && !string.IsNullOrEmpty(dbObj.FirstMeetingOwner) ? $"{meeting.MeetingType} meeting first meeting owner has been removed by {Environment.UserName}" :
                   $"{meeting.MeetingType} meeting first meeting owner has been updated by {Environment.UserName}. Changed '{dbObj.FirstMeetingOwner}' into '{meeting.FirstMeetingOwner}'";

                var tl2 = new Timeline().Create(meeting.RespondentID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    if (tl2.EmployeeID != null) timelineString.Append($"{tl2.GetHeader()} VALUES {tl2.GetValues()}");
                    haveUpdate = true;
                }
                else
                {
                    if (tl2.EmployeeID != null) timelineString.Append($",{tl2.GetValues()}");
                }

            }
            if (meeting.SecondMeetingOwner != dbObj.SecondMeetingOwner)
            {
                var message = string.IsNullOrEmpty(dbObj.SecondMeetingOwner) && !string.IsNullOrEmpty(meeting.SecondMeetingOwner) ? $"{meeting.MeetingType} meeting second meeting owner ({meeting.SecondMeetingOwner}) has been recorded by {Environment.UserName}" :
                    string.IsNullOrEmpty(meeting.SecondMeetingOwner) && !string.IsNullOrEmpty(dbObj.SecondMeetingOwner) ? $"{meeting.MeetingType} meeting second meeting owner has been removed by {Environment.UserName}" :
                   $"{meeting.MeetingType} meeting second meeting owner has been updated by {Environment.UserName}. Changed '{dbObj.SecondMeetingOwner}' into '{meeting.SecondMeetingOwner}'";

                var tl2 = new Timeline().Create(meeting.RespondentID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {   
                    if (tl2.EmployeeID != null) timelineString.Append($"{tl2.GetHeader()} VALUES {tl2.GetValues()}");
                    haveUpdate = true;
                }
                else
                {
                    if (tl2.EmployeeID != null) timelineString.Append($",{tl2.GetValues()}");
                }

            }

            if (meeting.FirstMeetingHRSupport != dbObj.FirstMeetingHRSupport)
            {
                var message = string.IsNullOrEmpty(dbObj.FirstMeetingHRSupport) && !string.IsNullOrEmpty(meeting.FirstMeetingHRSupport) ? $"{meeting.MeetingType} meeting first meeting HR support ({meeting.FirstMeetingHRSupport}) has been recorded by {Environment.UserName}" :
                    string.IsNullOrEmpty(meeting.FirstMeetingHRSupport) && !string.IsNullOrEmpty(dbObj.FirstMeetingHRSupport) ? $"{meeting.MeetingType} meeting first meeting HR support has been removed by {Environment.UserName}" :
                   $"{meeting.MeetingType} meeting first meeting HR support has been updated by {Environment.UserName}. Changed '{dbObj.FirstMeetingHRSupport}' into '{meeting.FirstMeetingHRSupport}'";

                var tl2 = new Timeline().Create(meeting.RespondentID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    if (tl2.EmployeeID != null) timelineString.Append($"{tl2.GetHeader()} VALUES {tl2.GetValues()}");
                    haveUpdate = true;
                }
                else
                {
                    if (tl2.EmployeeID != null) timelineString.Append($",{tl2.GetValues()}");
                }

            }
            if (meeting.SecondMeetingHRSupport != dbObj.SecondMeetingHRSupport)
            {
                var message = string.IsNullOrEmpty(dbObj.SecondMeetingHRSupport) && !string.IsNullOrEmpty(meeting.SecondMeetingHRSupport) ? $"{meeting.MeetingType} meeting second meeting HR support ({meeting.SecondMeetingHRSupport}) has been recorded by {Environment.UserName}" :
                    string.IsNullOrEmpty(meeting.SecondMeetingHRSupport) && !string.IsNullOrEmpty(dbObj.SecondMeetingHRSupport) ? $"{meeting.MeetingType} meeting second meeting HR support has been removed by {Environment.UserName}" :
                   $"{meeting.MeetingType} meeting second meeting HR support has been updated by {Environment.UserName}. Changed '{dbObj.SecondMeetingHRSupport}' into '{meeting.SecondMeetingHRSupport}'";

                var tl2 = new Timeline().Create(meeting.RespondentID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    if (tl2.EmployeeID != null) timelineString.Append($"{tl2.GetHeader()} VALUES {tl2.GetValues()}");
                    haveUpdate = true;
                }
                else
                {
                    if (tl2.EmployeeID != null) timelineString.Append($",{tl2.GetValues()}");
                }

            }

            if (meeting.FirstMeetingOutcome != dbObj.FirstMeetingOutcome)
            {
                var message = string.IsNullOrEmpty(dbObj.FirstMeetingOutcome) && !string.IsNullOrEmpty(meeting.FirstMeetingOutcome) ? $"{meeting.MeetingType} meeting first meeting outcome ({meeting.FirstMeetingOutcome}) has been recorded by {Environment.UserName}" :
                    string.IsNullOrEmpty(meeting.FirstMeetingOutcome) && !string.IsNullOrEmpty(dbObj.FirstMeetingOutcome) ? $"{meeting.MeetingType} meeting first meeting outcome has been removed by {Environment.UserName}" :
                   $"{meeting.MeetingType} meeting first meeting outcome has been updated by {Environment.UserName}. Changed '{dbObj.FirstMeetingOutcome}' into '{meeting.FirstMeetingOutcome}'";

                var tl2 = new Timeline().Create(meeting.RespondentID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    if (tl2.EmployeeID != null) timelineString.Append($"{tl2.GetHeader()} VALUES {tl2.GetValues()}");
                    haveUpdate = true;
                }
                else
                {
                    if (tl2.EmployeeID != null) timelineString.Append($",{tl2.GetValues()}");
                }

            }
            if (meeting.SecondMeetingOutcome != dbObj.SecondMeetingOutcome)
            {
                var message = string.IsNullOrEmpty(dbObj.SecondMeetingOutcome) && !string.IsNullOrEmpty(meeting.SecondMeetingOutcome) ? $"{meeting.MeetingType} meeting second meeting outcome ({meeting.SecondMeetingOutcome}) has been recorded by {Environment.UserName}" :
                    string.IsNullOrEmpty(meeting.SecondMeetingOutcome) && !string.IsNullOrEmpty(dbObj.SecondMeetingOutcome) ? $"{meeting.MeetingType} meeting second meeting outcome has been removed by {Environment.UserName}" :
                   $"{meeting.MeetingType} meeting second meeting outcome has been updated by {Environment.UserName}. Changed '{dbObj.SecondMeetingOutcome}' into '{meeting.SecondMeetingOutcome}'";

                var tl2 = new Timeline().Create(meeting.RespondentID, TimelineOrigin.Meetings, message);
                if (!haveUpdate)
                {
                    if (tl2.EmployeeID != null) timelineString.Append($"{tl2.GetHeader()} VALUES {tl2.GetValues()}");
                    haveUpdate = true;
                }
                else
                {
                    if (tl2.EmployeeID != null) timelineString.Append($",{tl2.GetValues()}");
                }

            }

            return haveUpdate ? timelineString.ToString() : string.Empty;
        }

        private void Automate(MeetingsEntity meeting, MeetingsEntity dbMeeting, AutomationAction action)
        {
            Task.Run(() =>
            {
                var automation = new MeetingsAutomation(this).SetData(dbMeeting, meeting);
                Task.Delay(1000);
                automation.Invoke(action);
            });
        }

        private void AutomateCustom(CustomMeetingEntity meeting, CustomMeetingEntity dbMeeting, AutomationAction action)
        {
            Task.Run(() =>
            {
                var automation = new CustomMeetingsAutomation(this).SetData(dbMeeting, meeting);
                Task.Delay(1000);
                automation.Invoke(action);
            });
        }

    }
}
