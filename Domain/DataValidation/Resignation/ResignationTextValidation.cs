using Domain.Models.Resignations;
using Domain.Networking;
using Domain.Types;
using System;

namespace Domain.DataValidation.Resignation
{
    public sealed class ResignationTextValidation : BaseValidationHandler<ResignationEntity>
    {
        public override void Validate(ValidationRequest<ResignationEntity> validation)
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

            if (validation.Data.LastWorkingDay.Equals(DateTime.MinValue))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Last Working Day is mandatory!"
                };
                return;
            }

            if (string.IsNullOrEmpty(validation.Data.ReasonForResignation))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Resignation reason is mandatory!"
                };
                return;
            }

            if (string.IsNullOrEmpty(validation.Data.TTLink))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "TT reference is mandatory!"
                };
                return;
            }

            if (!WebHelper.IsLink(validation.Data.TTLink))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Invalid resignation TT link"
                };
                return;
            }

            if (_nextHandler != null) _nextHandler.Validate(validation);
        }
    }
}
