using Domain.Factory;
using Domain.Models;
using Domain.Models.Meetings;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public class MeetingsRepository : BaseRepository
    {
        public Task<Response> InsertAllAsync(IList<IDataImportObject> awalList) => base.InsertAllAsync(awalList, "meetings");

        public Task<IEnumerable<MeetingsEntity>> GetEmployeeMeetingsAsync(string emplId) => GetCachedAsync<MeetingsEntity>($"SELECT * FROM meetings WHERE employeeID = '{emplId}' ORDER BY createdAt DESC");

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

        public Task<Response> CloseERMeeting(ref MeetingsEntity meeting, string closureReason)
        {
            var timeLine = new Timeline().Create(meeting.EmployeeID, TimelineOrigin.Meetings, $" ER meeting (ID: {meeting.ID}) has been cancelled by {Environment.UserName} due to '{closureReason}'");
            if (meeting.SecondMeetingDate != DateTime.MinValue) meeting.SecondMeetingOutcome = "Cancelled";
            if (meeting.SecondMeetingDate == DateTime.MinValue) meeting.FirstMeetingOutcome = "Cancelled";
            meeting.MeetingStatus = "Cancelled";
            var tlQuery = $"INSERT INTO timeline {timeLine.GetHeader()} VALUES {timeLine.GetValues()};";
            var query = $"UPDATE meetings SET meetingStatus = '{meeting.MeetingStatus}', firstMeetingOutcome = '{meeting.FirstMeetingOutcome}', SecondMeetingOutcome = '{meeting.SecondMeetingOutcome}' WHERE id = '{meeting.ID}'; {tlQuery}";
            return ExecuteAsync(query);
        }

    }
}
