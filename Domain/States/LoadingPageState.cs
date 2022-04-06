using System;

namespace Domain.Types
{
    [Flags]
    public enum LoadingPageState
    {
        Default = 0x0000,
        LoadingWidgetOn = 0x0001,
        LoadingWidgetOff = 0x0002,
        SettingsLoading = 0x0004 + LoadingWidgetOn,
        SettingsFailedToLoad = 0x0008,
        RosterLoading = 0x0010 + LoadingWidgetOn,
        RosterFailedToLoad = 0x0020,
        DbLoading = 0x0040 + LoadingWidgetOn,
        DbPathFailedToLoad = 0x0080,
        DbFailedToLoad = 0x0100,
        CasheDbLoading = 0x0200 + LoadingWidgetOn,
        CasheDbFailedToLoad = 0x0400
    }
}
