using Domain.Models.CustomMeetings;
using Domain.Models.Meetings;
using Domain.Types;

namespace Domain.DataValidation.Meetings
{
    internal sealed class MeetingsValidation : IDataValidation
    {
        public Response Validate<T>(T meeting)
        {
            if (meeting is MeetingsEntity) return MeetingValidation(meeting as MeetingsEntity);
            if (meeting is CustomMeetingEntity) return CustomMeetingValidation(meeting as CustomMeetingEntity);

            return new Response { Success = false, Message = "Failed to identify meeting type" };
        }

        private Response MeetingValidation(MeetingsEntity meeting)
        {
            var validationRequest = new ValidationRequest<MeetingsEntity> { Data = meeting };
            var erMeetingValidation = new ERMeetingValidation();

            erMeetingValidation.Validate(validationRequest);

            return validationRequest.ValidationResponse;
        }

        private Response CustomMeetingValidation(CustomMeetingEntity meeting)
        {
            var validationRequest = new ValidationRequest<CustomMeetingEntity> { Data = meeting };
            var erMeetingValidation = new CustomMeetingValidation();

            erMeetingValidation.Validate(validationRequest);

            return validationRequest.ValidationResponse;
        }
    }
}
