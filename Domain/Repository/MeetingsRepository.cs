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

        public Task<Response> InsertAllAsync(IList<IDataImportObject> awalList) => base.InsertAllAsync(awalList, "meetings");

        public Task<Response> InsertCustomAllAsync(IList<IDataImportObject> awalList) => base.InsertAllAsync(awalList, "custom_meetings");

        public Task<IEnumerable<MeetingsEntity>> GetEmployeeMeetingsAsync(string emplId) => 
            GetCachedAsync<MeetingsEntity>($"SELECT * FROM meetings WHERE employeeID = '{emplId}' ORDER BY createdAt DESC");

        public Task<IEnumerable<CustomMeetingEntity>> GetEmployeeCustomMeetingsAsync(string emplId) =>
            GetCachedAsync<CustomMeetingEntity>($"SELECT * FROM custom_meetings WHERE claimantID = '{emplId}' OR respondentID = '{emplId}' ORDER BY createdAt DESC");

        public Task<Response> InsertAsync(MeetingsEntity meeting)
        {
            if (IsErMeetingExists(meeting.ID)) return Task.Run(() => new Response { Success = false, Message = "ER case with this ID already exists" });

            var timeLine = new Timeline().Create(meeting.EmployeeID, TimelineOrigin.Meetings, $" ER meeting (type - {meeting.MeetingType}) has been created by {meeting.CreatedBy}. Case ID: {meeting.ID}");
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";

            var query = $"INSERT INTO meetings {meeting.GetHeader()} VALUES {meeting.GetValues()}; {tlQuery}";
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
                timeLine2 = timeLine2.Create(meeting.RespondentID, TimelineOrigin.CustomMeetings, $"{meeting.MeetingType} meeting has been cancelled by {Environment.UserName} due to '{closureReason}'");
                tl2Query = $",{timeLine2.GetValues()}";
            }
            var timeLine = new Timeline().Create(emplId, TimelineOrigin.CustomMeetings, $"{meeting.MeetingType} meeting has been cancelled by {Environment.UserName} due to '{closureReason}'");
            if (meeting.SecondMeetingDate != DateTime.MinValue) meeting.SecondMeetingOutcome = "Cancelled";
            if (meeting.SecondMeetingDate == DateTime.MinValue) meeting.FirstMeetingOutcome = "Cancelled";

            meeting.MeetingStatus = "Cancelled";
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
                        secondMeetingDate = {meeting.FirstMeetingDate.DbNullableSanityCheck(DataStorage.ShortDBDateFormat)}, secondMeetingOutcome = '{meeting.SecondMeetingOutcome}', updatedBy = '{Environment.UserName}', 
                        updatedAt = '{meeting.UpdatedAt.ToString(DataStorage.LongDBDateFormat)}', meetingStatus = '{meeting.MeetingStatus}', paperless = '{Convert.ToInt16(meeting.Paperless)}' 
                        WHERE id = '{meeting.ID}'; {timelineQuery}";

           
            Automate(meeting, dbMeeting, AutomationAction.OnUpdate);
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

    }
}
