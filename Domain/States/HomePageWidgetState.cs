using System;

namespace Domain.Types
{
    [Flags]
    public enum HomePageWidgetState
    {
        Default = 0x0000,
        SummaryLoading = 0x0001,
        SummaryLoaded = 0x0002,
        EmployeeStatusLoading = 0x0004,
        EmployeeStatusLoaded = 0x0008,
        EmployeeAwalSummaryLoading = 0x0010,
        EmployeeAwalSummaryLoaded = 0x0020,
        EmployeeSanctionsLoading = 0x0040,
        EmployeeSanctionsLoaded = 0x0080,
        EmployeeERMeetingsLoading = 0x0100,
        EmployeeERMeetingsLoaded = 0x0200,
        EmployeeCustomMeetingsLoading = 0x0400,
        EmployeeCustomMeetingsLoaded = 0x0800,
        EmployeeTimelineLoading = 0x1000,
        EmployeeTimelineLoaded = 0x2000,
        EmployeeCommentsLoading = 0x4000,
        EmployeeCommentsLoaded = 0x8000

    }
}
