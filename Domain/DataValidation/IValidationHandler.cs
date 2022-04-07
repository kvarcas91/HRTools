namespace Domain.DataValidation
{
    public interface IValidationHandler<T>
    {
        void SetNextHandler(IValidationHandler<T> handler);
        void Validate(ValidationRequest<T> data);
    }
}
