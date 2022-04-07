using Domain.Models.AWAL;
using Domain.Types;
using System;

namespace Domain.DataValidation
{
    internal class AwalValidatiosn 
    {

        //public override Response Validate<T>(T value)
        //{
        //    var response = base.Validate(value);
        //    if (!response.Success) return response;

        //    var awalEntry = value as AwalEntity;

        //    if (awalEntry.FirstNCNSDate.Equals(DateTime.MinValue))
        //    {
        //        return new Response
        //        {
        //            Success = false,
        //            Message = "1st NCNS field is mandatory"
        //        };
        //    }

        //    if ((awalEntry.FirstNCNSDate != DateTime.MinValue &&  awalEntry.FirstNCNSDate > DateTime.Now) ||
        //        (awalEntry.Awal1SentDate != DateTime.MinValue && awalEntry.Awal1SentDate > DateTime.Now) ||
        //        (awalEntry.Awal2SentDate != DateTime.MinValue && awalEntry.Awal2SentDate > DateTime.Now))
        //    {
        //        return new Response
        //        {
        //            Success = false,
        //            Message = "first NCNS date or AWAL letter sent date cannot be set in the future date"
        //        };
        //    }

        //    if (awalEntry.Awal1SentDate == DateTime.MinValue && awalEntry.Awal2SentDate != DateTime.MinValue)
        //    {
        //        return new Response
        //        {
        //            Success = false,
        //            Message = "AWAL 2 sent date cannot be set before AWAL 1 sent date"
        //        };
        //    }

        //    return response;
        //}

    }
}
