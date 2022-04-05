using Domain.Types;
using Prism.Events;


namespace HRTools_v2.Args
{
    public class ShowToastArgs : PubSubEvent<(string, NotificationType)>
    {
    }
}
