using Domain.Models.Meetings;
using Domain.Types;
using System;

namespace Domain.DataValidation.Meetings
{
    internal class ERMeetingValidation : BaseValidationHandler<MeetingsEntity>
    {
        public override void Validate(ValidationRequest<MeetingsEntity> validation)
        {

            if (validation.Data == null)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "No Object found"
                };
                return;
            }

            if (validation.Data.FirstMeetingDate == DateTime.MinValue)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "1st meeting date is mandatory"
                };
                return;
            }

            if (validation.Data.SecondMeetingDate != DateTime.MinValue && string.IsNullOrEmpty(validation.Data.FirstMeetingOutcome))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Please set first meeting outcome"
                };
                return;
            }

            if (validation.Data.SecondMeetingDate == DateTime.MinValue && !string.IsNullOrEmpty(validation.Data.SecondMeetingOutcome))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Please set second meeting date"
                };
                return;
            }

            if (_nextHandler != null) _nextHandler.Validate(validation);
        }
    }
}
