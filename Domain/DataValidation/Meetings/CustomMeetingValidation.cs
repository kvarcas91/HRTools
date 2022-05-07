using Domain.Models.CustomMeetings;
using Domain.Storage;
using Domain.Types;
using System;
using System.Linq;

namespace Domain.DataValidation.Meetings
{
    internal class CustomMeetingValidation : BaseValidationHandler<CustomMeetingEntity>
    {
        public override void Validate(ValidationRequest<CustomMeetingEntity> validation)
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

            if (validation.Data.FirstMeetingDate == DateTime.MinValue && !string.IsNullOrEmpty(validation.Data.FirstMeetingOutcome))
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

            if (!string.IsNullOrEmpty(validation.Data.FirstMeetingOutcome) && string.IsNullOrEmpty(validation.Data.FirstMeetingHRSupport))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "1st meeting HR support is mandatory"
                };
                return;
            }

            if (!string.IsNullOrEmpty(validation.Data.FirstMeetingOutcome) && string.IsNullOrEmpty(validation.Data.FirstMeetingOwner))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "1st meeting owner (AM/OM) is mandatory"
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

            if (!string.IsNullOrEmpty(validation.Data.SecondMeetingOutcome) && string.IsNullOrEmpty(validation.Data.SecondMeetingHRSupport))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "2nd meeting HR support is mandatory"
                };
                return;
            }

            if (!string.IsNullOrEmpty(validation.Data.SecondMeetingOutcome) && string.IsNullOrEmpty(validation.Data.SecondMeetingOwner))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "2nd meeting owner (AM/OM) is mandatory"
                };
                return;
            }

            var firstHRSupport = DataStorage.RosterList.Where(x => x.UserID == validation.Data.FirstMeetingHRSupport.Trim()).FirstOrDefault();
            if (!string.IsNullOrEmpty(validation.Data.FirstMeetingHRSupport) && firstHRSupport == null)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Invalid first HR support login"
                };
                return;
            }

            var secondHRSupport = DataStorage.RosterList.Where(x => x.UserID == validation.Data.SecondMeetingHRSupport.Trim()).FirstOrDefault();
            if (!string.IsNullOrEmpty(validation.Data.SecondMeetingHRSupport) && secondHRSupport == null)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Invalid second HR support login"
                };
                return;
            }

            var firstOwner = DataStorage.RosterList.Where(x => x.UserID == validation.Data.FirstMeetingOwner.Trim()).FirstOrDefault();
            if (!string.IsNullOrEmpty(validation.Data.FirstMeetingOwner) && firstOwner == null)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Invalid first meeting owner login"
                };
                return;
            }

            var secondOwner = DataStorage.RosterList.Where(x => x.UserID == validation.Data.SecondMeetingOwner.Trim()).FirstOrDefault();
            if (!string.IsNullOrEmpty(validation.Data.SecondMeetingOwner) && secondOwner == null)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Invalid second meeting owner login"
                };
                return;
            }

            if (_nextHandler != null) _nextHandler.Validate(validation);
        }
    }
}
