using Domain.Models.Resignations;
using Domain.Networking;
using Domain.Types;
using System;

namespace Domain.DataValidation
{
    internal class ResignationValidator : DataValidator
    {
        public override Response Validate<T>(T value)
        {
            var response = base.Validate(value);
            if (!response.Success) return response;
            
            var resignationEntry = value as ResignationEntity;

            if (string.IsNullOrEmpty(resignationEntry.TTLink) || string.IsNullOrEmpty(resignationEntry.ReasonForResignation) || 
                resignationEntry.LastWorkingDay.Equals(DateTime.MinValue))
            {
                return new Response
                {
                    Success = false,
                    Message = "All fields are mandatory"
                };
            }

            if (!WebHelper.IsLink(resignationEntry.TTLink))
            {
                return new Response
                {
                    Success = false,
                    Message = "Invalid resignation TT link"
                };
            }

            return response;

        }
    }
}
