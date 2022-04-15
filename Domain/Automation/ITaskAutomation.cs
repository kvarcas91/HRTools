using Domain.Types;

namespace Domain.Automation
{
    public interface ITaskAutomation<T>
    {
        void Invoke(AutomationAction action);
        ITaskAutomation<T> SetData(T oldObj, T newObj);
    }
}
