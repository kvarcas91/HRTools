using Domain.Types;

namespace Domain.DataValidation
{
    public class ValidationRequest<T>
    {
        public T Data { get; set; }
        public Response ValidationResponse;

        public ValidationRequest()
        {
            ValidationResponse = new Response { Success = true};
        }
    }
}
