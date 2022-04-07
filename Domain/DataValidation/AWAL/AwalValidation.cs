using Domain.Models.AWAL;
using Domain.Types;

namespace Domain.DataValidation.AWAL
{
    internal sealed class AwalValidation : IDataValidation
    {
        public Response Validate<T>(T awal)
        {
            var awalEntity = awal as AwalEntity;
            var validationRequest = new ValidationRequest<AwalEntity> { Data = awalEntity };
            var dateValidation = new AwalDateValidation();

            dateValidation.Validate(validationRequest);

            return validationRequest.ValidationResponse;
        }
    }
}
