using Domain.Models.AWAL;
using Domain.Types;
using System;

namespace Domain.DataValidation.AWAL
{
    internal sealed class AwalDateValidation : BaseValidationHandler<AwalEntity>
    {
        public override void Validate(ValidationRequest<AwalEntity> validation)
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

            if (validation.Data.FirstNCNSDate == DateTime.MinValue)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "1st NCNS field is mandatory"
                };
                return;
            }

            if (validation.Data.FirstNCNSDate > DateTime.Now)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "First NCNS date sent date cannot be set in the future!"
                };
                return;
            }

            if (validation.Data.Awal1SentDate > DateTime.Now || validation.Data.Awal2SentDate > DateTime.Now)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "AWAL sent date cannot be set in the future!"
                };
                return;
            }

            if (validation.Data.Awal1SentDate != DateTime.MinValue && validation.Data.Awal1SentDate < validation.Data.FirstNCNSDate)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "AWAL 1 sent date cannot be before first NCNS date!"
                };
                return;
            }

            if (validation.Data.Awal2SentDate != DateTime.MinValue && validation.Data.Awal2SentDate < validation.Data.Awal1SentDate)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "AWAL 2 sent date cannot be before AWAL 1 sent date!"
                };
                return;
            }

            var IsOnProbation = (DateTime.Now - validation.Data.EmploymentStartDate).Days < 90;

            if (validation.Data.Awal1SentDate != DateTime.MinValue && validation.Data.Awal1SentDate < validation.Data.FirstNCNSDate.AddDays(1) &&
                !IsOnProbation)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "AWAL 1 cannot be sent today, as AA is outside probation period!"
                };
                return;
            }

            if (validation.Data.Awal2SentDate != DateTime.MinValue && validation.Data.Awal2SentDate < validation.Data.Awal1SentDate.AddDays(6) &&
                !IsOnProbation)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "AWAL 2 cannot be sent today, as it must be sent ONLY after 5 days from the date when AWAL 1 was sent!"
                };
                return;
            }

            if (validation.Data.DisciplinaryDate != DateTime.MinValue && validation.Data.DisciplinaryDate <= validation.Data.Awal2SentDate)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "Disciplinary date cannot be set before AWAL 2 sent date!"
                };
                return;
            }

            if (validation.Data.Awal2SentDate != DateTime.MinValue && validation.Data.DisciplinaryDate == DateTime.MinValue)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "If AWAL 2 letter is sent, please update Disciplinary date!"
                };
                return;
            }

            if (validation.Data.DisciplinaryDate != DateTime.MinValue && (validation.Data.Awal1SentDate == DateTime.MinValue || 
                validation.Data.Awal2SentDate == DateTime.MinValue))
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "AWAL sent date is mandatory!"
                };
                return;
            }

            if (!string.IsNullOrEmpty(validation.Data.Outcome) && !validation.Data.Outcome.Equals("Cancelled") && validation.Data.DisciplinaryDate == DateTime.MinValue)
            {
                validation.ValidationResponse = new Response
                {
                    Success = false,
                    Message = "If there is an outcome, please update Disciplinary date!"
                };
                return;
            }

            if (_nextHandler != null) _nextHandler.Validate(validation);
        }
    }
}
