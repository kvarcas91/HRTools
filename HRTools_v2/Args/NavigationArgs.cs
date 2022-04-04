using Domain.Models;
using Prism.Events;

namespace HRTools_v2.Args
{
    public class NavigationArgs : PubSubEvent<string>
    {
    }

    public class NavigationEmplArgs : PubSubEvent<(string, Roster)>
    {
    }
}
