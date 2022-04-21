using Domain.Models.Meetings;
using Domain.Types;

namespace Domain.DataValidation.Meetings
{
    internal sealed class MeetingsValidation : IDataValidation
    {
        public Response Validate<T>(T meeting)
        {
            var meetingEntity = meeting as MeetingsEntity;
            var validationRequest = new ValidationRequest<MeetingsEntity> { Data = meetingEntity };
            var erMeetingValidation = new ERMeetingValidation();

            erMeetingValidation.Validate(validationRequest);

            return validationRequest.ValidationResponse;
        }
    }
}
