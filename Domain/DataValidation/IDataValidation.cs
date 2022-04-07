using Domain.Types;

namespace Domain.DataValidation
{
    internal interface IDataValidation
    {
        Response Validate<T>(T data);
    }
}
