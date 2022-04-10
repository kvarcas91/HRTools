using System.Collections.Generic;

namespace Domain.Factory
{
    public interface IDataMap
    {
        Dictionary<string, string> GetRequiredHeaders();
    }
}
