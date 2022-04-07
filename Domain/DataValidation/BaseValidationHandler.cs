using System;

namespace Domain.DataValidation
{
    public class BaseValidationHandler<T> : IValidationHandler<T>
    {
        public IValidationHandler<T> _nextHandler;

        public BaseValidationHandler()
        {
            _nextHandler = null;
        }

        public void SetNextHandler(IValidationHandler<T> handler)
        {
            _nextHandler = handler;
        }

        public virtual void Validate(ValidationRequest<T> validation)
        {
            throw new NotImplementedException();
        }
    }
}
