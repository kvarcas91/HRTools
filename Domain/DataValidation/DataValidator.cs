using Domain.Types;

namespace Domain.DataValidation
{
    internal abstract class DataValidator
    {
        public virtual Response Validate<T>(T value) where T : new()
        {
            if (value == null) return new Response { Message = "data object is null", Success = false };
            else return new Response { Success = true };
        }
    }
}
