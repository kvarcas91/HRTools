using System;

namespace Domain.Types
{
    [Flags]
    public enum TimelineOrigin
    {
        ALL = 0x001,
        AWAL = 0x002,
        Meetings = 0x004,
        Resignations = 0x008,
        Sanctions = 0x010,
        Suspensions = 0x020
    }
}
