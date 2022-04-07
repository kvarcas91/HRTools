using Domain.Models.Resignations;
using Domain.Types;

namespace Domain.DataValidation.Resignation
{
    internal class ResignationValidation : IDataValidation
    {
        public Response Validate<T>(T resignation)
        {
            var resignationEntity = resignation as ResignationEntity;
            var validationRequest = new ValidationRequest<ResignationEntity> { Data = resignationEntity };
            var textValidation = new ResignationTextValidation();

            textValidation.Validate(validationRequest);

            return validationRequest.ValidationResponse;
        }

    }
}
